using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace mbs
{
    // A rail that's used to transport the player.
    public class Rail : MonoBehaviour
    {
        // The speed of the rail.
        public float speed = 1.0F;

        // If set to 'true', the rail loops.
        public bool loopPoints = false;

        // The points on the rail that is used to transport the player.
        public List<GameObject> points = new List<GameObject>();

        // The objects attached to this rail.
        public List<RailRider> riders = new List<RailRider>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        private void OnCollisionEnter(Collision collision)
        {
            // The rider component.
            RailRider rider;

            // Tries to grab the rider component.
            if(collision.gameObject.TryGetComponent(out rider))
            {
                AttachToRail(rider);
            }
        }

        // OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider
        private void OnCollisionStay(Collision collision)
        {
            // The rider component.
            RailRider rider;

            // Tries to grab the rider component.
            if (collision.gameObject.TryGetComponent(out rider))
            {
                // If the rider is in the list, detach them from the rail.
                if(riders.Contains(rider))
                    DetachFromRail(rider);
            }
        }


        // Attaches the provided entity to the rail.
        private void AttachToRail(RailRider rider)
        {
            // Checks if ther there points to connect to.
            if (points.Count <= 1)
                return;

            // The closest point.
            GameObject closestPoint = points[0];
            float closestDist = Vector3.Distance(closestPoint.transform.position, rider.transform.position);

            // Goes through all the points.
            foreach (GameObject point in points)
            {
                float currDist = (point.transform.position - rider.transform.position).magnitude;

                // Save the closest point and closest distance.
                if (currDist <= closestDist)
                {
                    closestPoint = point;
                    closestDist = currDist;
                }
            }


            // Calculates if the cloest point should be the start point or end point.

            // Gets the index of the closest point.
            int closestIndex = points.IndexOf(closestPoint);

            // Checks the cloest index.
            if (closestIndex > 0) // Start of the rail.
            {
                rider.startPoint = closestPoint;
                rider.endPoint = points[closestIndex + 1];
            }
            else if (closestIndex == points.Count - 1) // End of the rail.
            {
                rider.startPoint = points[closestIndex - 1];
                rider.endPoint = closestPoint;
            }
            else // Some point along the rail.
            {
                // Gets the previous point and next point.
                GameObject prevPoint = points[closestIndex - 1];
                GameObject nextPoint = points[closestIndex + 1];

                // Gets the distance to the previous point and next point.
                float prevDist = Vector3.Distance(rider.transform.position, prevPoint.transform.position);
                float nextDist = Vector3.Distance(rider.transform.position, nextPoint.transform.position);

                // Checks the minimum of the two distances.
                if (Mathf.Min(prevDist, nextDist) == prevDist) // Closest to previous.
                {
                    rider.startPoint = prevPoint;
                    rider.endPoint = closestPoint;
                }
                else // Closest to next.
                {
                    rider.startPoint = closestPoint;
                    rider.endPoint = nextPoint;
                }
            }

            // Calculates the t-value along the x, y, and z.
            float tx = Mathf.InverseLerp(rider.startPoint.transform.position.x, rider.endPoint.transform.position.x, rider.transform.position.x);
            float ty = Mathf.InverseLerp(rider.startPoint.transform.position.y, rider.endPoint.transform.position.y, rider.transform.position.y);
            float tz = Mathf.InverseLerp(rider.startPoint.transform.position.z, rider.endPoint.transform.position.z, rider.transform.position.z);

            // Puts the t-values into an array.
            float[] tArr = { tx, ty, tz };

            // The sum of the t-values.
            float tSum = 0;

            // Number of points in the array.
            int count = 0;

            // Goes through each component of the start and end point positions.
            for (int i = 0; i < 3; i++)
            {
                // If the points are different, add the value to the sum.
                if (rider.startPoint.transform.position[i] != rider.endPoint.transform.position[i])
                {
                    tSum += tArr[i];
                    count++;
                }
            }

            // If the count is greater than 0, average out for the t-value.
            if (count != 0)
            {
                // Calculate the rail t value.
                rider.railT = tSum / count;
            }
            else // The two points perfectly overlap.
            {
                // Just set the value to 0.0F.
                rider.railT = 0;
            }

            // Checks if the rider.railT is beyond the set points.
            // If it is, then move onto either the next or previous rail segment.
            if (rider.railT > 1) // Move onto next point since it's greater than 1.
            {
                rider.startPoint = closestPoint;
                rider.endPoint = (closestIndex < points.Count - 1) ? points[closestIndex + 1] : null;
                rider.railT -= 1;
            }
            else if (rider.railT < 0) // Move onto previous point since it's less than 1.
            {
                rider.startPoint = (closestIndex > 0) ? points[closestIndex - 1] : null;
                rider.endPoint = closestPoint;
                rider.railT += 1;
            }

            // Clamp the vlaue so that it's between (0, 1).
            rider.railT = Mathf.Clamp01(rider.railT);

            // Add to attached objects.
            riders.Add(rider);
            rider.rail = this;
        }

        // Detaches the provided rider from the rail.
        // distFromLast: the distance from the previous point on the rail.
        private void DetachFromRail(RailRider rider, Vector3 oldPos)
        {
            // Remove from list.
            if(riders.Contains(rider))
                riders.Remove(rider);

            // Clear out the values.
            rider.rail = null;
            rider.startPoint = null;
            rider.endPoint = null;

            // The rail t-value is now 0.
            rider.railT = 0.0F;

            // TODO: maybe keep it attached to the rail, but don't have it move.

            // If the rider doesn't have a rigidbody
            if(rider.rigidbody != null)
            {
                // Calculates the force after being pushed off.
                Vector3 force = (rider.transform.position - oldPos) * Vector3.Distance(rider.transform.position, oldPos);
                rider.rigidbody.AddForce(force, ForceMode.Impulse);
            }
        }

        // Detaches the rider from the rail (does not push off).
        private void DetachFromRail(RailRider rider)
        {
            DetachFromRail(rider, rider.transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: create system for automatically travelling across the rail.
            // TODO: make sure the user goes the same speed across the whole rail.

            // IF the rail has points to travel along.
            if(points.Count > 1)
            {
                // Goes through all riders.
                foreach (RailRider rider in riders)
                {
                    // TODO: perform catmull-rom calculation

                    // Increase t, and clamp it.
                    rider.railT += Time.deltaTime * speed * rider.speed;
                    rider.railT = Mathf.Clamp01(rider.railT);

                    // The previous position of the rider.
                    Vector3 riderOldPos = rider.transform.position;

                    // Update the postion.
                    rider.transform.position =
                        Vector3.Lerp(
                            rider.startPoint.transform.position,
                            rider.endPoint.transform.position,
                            rider.railT);

                    // Zeroes out the rider's velocity.
                    if (rider.rigidbody != null)
                    {
                        rider.rigidbody.velocity = Vector3.zero;
                    }

                    // End of rail segment.
                    if (rider.railT >= 1.0F)
                    {
                        // Gets the end index.
                        int endIndex = points.IndexOf(rider.endPoint);

                        // The end index is valid.
                        if (endIndex >= 0 && endIndex < points.Count)
                        {
                            // Checks if this is the last point in the list.
                            if ((endIndex + 1) < points.Count) // Not last point.
                            {
                                rider.startPoint = rider.endPoint;
                                rider.endPoint = points[endIndex + 1];
                                rider.railT = 0.0F;
                            }
                            else // Last point.
                            {
                                // If the rail loops, loop back to the start.
                                if (loopPoints)
                                {
                                    rider.startPoint = rider.endPoint;
                                    rider.endPoint = points[0];
                                    rider.railT = 0.0F;
                                }
                                else // Doesn't loop.
                                {
                                    DetachFromRail(rider, riderOldPos);
                                }
                            }
                        }
                        else
                        {
                            // Release from the rail.
                            DetachFromRail(rider, riderOldPos);
                        }
                    }

                    // The position has been updated.
                    rider.OnPositionUpdated();
                }
            }
            
        }
    }
}