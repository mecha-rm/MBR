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
        /*
         * The mode of the rail.
         * Linear: straight path from point to point.
         * Bezier: curved approach to points using the bezier method.
         * Catmull-Rom: curved appraoch to points using the catmull-rom meethod. 
         */
        public enum railMode { linear, bezier, catmullRom }

        // The tag for rails.
        public static string RAIL_TAG = "Rail";

        // The points on the rail that is used to transport the player.
        public List<RailWaypoint> waypoints = new List<RailWaypoint>();

        // The objects attached to this rail.
        public List<RailRider> riders = new List<RailRider>();

        // The movement mode of the rail.
        [Tooltip("The movement mode of the rail.")]
        public railMode mode = railMode.linear;

        // The speed of the rail.
        [Tooltip("The speed of the rail.")]
        public float speed = 10.0F;

        // The number of samples for curved rail modes.
        [Tooltip("The number of samples for curved movement. This has no effect if linear mode is used.")]
        public int samples = 11;

        // If set to 'true', the rail loops.
        [Tooltip("If true, the rail loops back to the start.")]
        public bool loopRail = false;


        // Start is called before the first frame update
        void Start()
        {

        }

        // OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        private void OnCollisionEnter(Collision collision)
        {
            TryAttachToRail(collision.gameObject);
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            TryDetachFromRail(collision.gameObject);
        }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider collision)
        {
            TryAttachToRail(collision.gameObject);
        }

        // OnTriggerExit is called when the Collider other has stopped touching the trigger.
        private void OnTriggerExit(Collider collision)
        {
            TryDetachFromRail(collision.gameObject);
        }



        // Attemps to attach the game object to the rail (only works if game object has RailRider script attached).
        public bool TryAttachToRail(GameObject entity)
        {
            // The rider component.
            RailRider rider;

            // Tries to grab the rider component.
            if (entity.gameObject.TryGetComponent(out rider))
            {
                // If the rider isn't in the list, try to attach it.
                if(!riders.Contains(rider) || rider.rail != this)
                    AttachToRail(rider);
            }

            // Checks if in list.
            bool added = riders.Contains(rider);
            return added;
        }


        // Attaches the provided entity to the rail.
        private void AttachToRail(RailRider rider)
        {
            // Calculates the start and end poitns for the rider.
            CalculateStartAndEndPoints(rider);

            // Add to attached objects.
            if(!riders.Contains(rider))
                riders.Add(rider);
            
            rider.rail = this;
            rider.transform.rotation = Quaternion.identity;

            // On attach to the rail.
            rider.OnAttachToRail(this);
        }

        // Calculates the start and end points based on the position of the rider.
        private void CalculateStartAndEndPoints(RailRider rider)
        {
            // Checks if ther there points to connect to.
            if (waypoints.Count <= 1)
                return;

            // The closest point.
            RailWaypoint closestPoint = waypoints[0];
            float closestDist = Vector3.Distance(closestPoint.transform.position, rider.transform.position);

            // Goes through all the points.
            foreach (RailWaypoint point in waypoints)
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
            int closestIndex = waypoints.IndexOf(closestPoint);

            // Checks the cloest index.
            if (closestIndex >= 0 && closestIndex < waypoints.Count - 1) // Start of the rail.
            {
                rider.startPoint = closestPoint;
                rider.endPoint = waypoints[closestIndex + 1];
            }
            else if (closestIndex == waypoints.Count - 1) // End of the rail.
            {
                rider.startPoint = waypoints[closestIndex - 1];
                rider.endPoint = closestPoint;
            }
            else // Some point along the rail.
            {
                // Gets the previous point and next point.
                RailWaypoint prevPoint = waypoints[closestIndex - 1];
                RailWaypoint nextPoint = waypoints[closestIndex + 1];

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

            // LERP CALCULATION (OLD)
            // // If the count is greater than 0, average out for the t-value.
            // if (count != 0)
            // {
            //     // Calculate the rail t value.
            //     rider.railT = tSum / count;
            // }
            // else // The two points perfectly overlap.
            // {
            //     // Just set the value to 0.0F.
            //     rider.railT = 0;
            // }
            // 
            // // Checks if the rider.railT is beyond the set points.
            // // If it is, then move onto either the next or previous rail segment.
            // if (rider.railT > 1) // Move onto next point since it's greater than 1.
            // {
            //     rider.startPoint = closestPoint;
            //     rider.endPoint = (closestIndex < points.Count - 1) ? points[closestIndex + 1] : null;
            //     rider.railT -= 1;
            // }
            // else if (rider.railT < 0) // Move onto previous point since it's less than 1.
            // {
            //     rider.startPoint = (closestIndex > 0) ? points[closestIndex - 1] : null;
            //     rider.endPoint = closestPoint;
            //     rider.railT += 1;
            // }
            // 
            // // Clamp the vlaue so that it's between (0, 1).
            // rider.railT = Mathf.Clamp01(rider.railT);

            // NEW - recalculate rail segment, but don't save T since moveTowards is being used.
            // The t-value of the rail, if used for a LERP calculation.
            float railT = 0.0F;

            // If the count is greater than 0, average out for the t-value.
            if (count != 0)
            {
                // Calculate the rail t value.
                railT = tSum / count;
            }
            else // The two points perfectly overlap.
            {
                // Just set the value to 0.0F.
                railT = 0;
            }
            
            // Checks if the railT is beyond the set points.
            // If it is, then move onto either the next or previous rail segment.
            if (railT > 1) // Move onto next point since it's greater than 1.
            {
                rider.startPoint = closestPoint;
                rider.endPoint = (closestIndex < waypoints.Count - 1) ? waypoints[closestIndex + 1] : null;
            }
            else if (railT < 0) // Move onto previous point since it's less than 1.
            {
                rider.startPoint = (closestIndex > 0) ? waypoints[closestIndex - 1] : null;
                rider.endPoint = closestPoint;
            }
        }


        // Tries to detach from the rail.
        public bool TryDetachFromRail(GameObject entity)
        {
            // The rider component.
            RailRider rider;

            // Tries to grab the rider component.
            if (entity.TryGetComponent(out rider))
            {
                // If the rider is in the list, detach them from the rail.
                if (riders.Contains(rider) || rider.rail != null)
                    DetachFromRail(rider);
            }

            // Checks to see that rider was removed.
            bool removed = !riders.Contains(rider);
            return removed;
        }

        // Detaches the provided rider from the rail.
        // distFromLast: the distance from the previous point on the rail.
        private void DetachFromRail(RailRider rider, Vector3 oldPos)
        {
            // Remove from list.
            if(riders.Contains(rider))
                riders.Remove(rider);

            // Clear out the values.
            // Clear out the rail.
            if(rider.rail != this && rider.rail != null)
            {
                // Remove the rider from its rail.
                if(rider.rail.riders.Contains(rider))
                    rider.rail.riders.Remove(rider);
            }

            // Clear rail.
            rider.rail = null;

            // TODO: mak sure the endpoint matches with where the entity detached from the rail.
            Vector3 endPointUp = rider.endPoint.transform.up;

            // Clear out start and end point.
            rider.startPoint = null;
            rider.endPoint = null;

            // The rail t-value is now 0 (no longer used).
            // rider.railT = 0.0F;

            // Makes sure that the rider has the same up as their endpoint.
            rider.transform.up = endPointUp;

            // TODO: maybe keep it attached to the rail, but don't have it move.

            // If the rider doesn't have a rigidbody (TODO: this doesn't appear to work at adding force).
            if(rider.rigidbody != null)
            {
                // Calculates the force after being pushed off. TODO: the multiple is by 10, which may need to be changed.
                Vector3 force = (rider.transform.position - oldPos) * Vector3.Distance(rider.transform.position, oldPos) * 10.0F;
                rider.rigidbody.AddForce(force, ForceMode.Impulse);
            }

            // On detach from the rail.
            rider.OnDetachFromRail(this);
        }

        // Detaches the rider from the rail (does not push off).
        private void DetachFromRail(RailRider rider)
        {
            DetachFromRail(rider, rider.transform.position);
        }

        // Checks if the rider is attached to the rail.
        public bool IsRiderAttached(RailRider rider)
        {
            bool result = riders.Contains(rider);
            return result;
        }


        // MOVEMENT

        // Returns the bezier position with the current position, startPoint, and endPoint.
        private Vector3 GetBezierPosition(RailRider rider, RailWaypoint startPoint, RailWaypoint endPoint)
        {
            // TODO: implement.
            return new Vector3();
        }

        // Returns the catmull-rom position with the current position, startPoint, and endPoint.
        private Vector3 GetCatmullRomPosition(RailRider rider, RailWaypoint p0, RailWaypoint p1, RailWaypoint p2, RailWaypoint p3)
        {
            // TODO: implement.
            return new Vector3();
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: create system for automatically travelling across the rail.
            // TODO: make sure the user goes the same speed across the whole rail.

            // IF the rail has points to travel along.
            if(waypoints.Count > 1 && riders.Count > 0)
            {
                // Goes through all riders.
                for(int i = riders.Count - 1; i >= 0; i--)
                {
                    // Get the rider.
                    RailRider rider = riders[i];

                    // TODO: perform catmull-rom calculation

                    // Attach to the rail again if no start point or end point are set.
                    if (rider.startPoint == null || rider.endPoint == null)
                        CalculateStartAndEndPoints(rider);

                    // Lerp - OLD
                    // // Increase t, and clamp it. - Old
                    // rider.railT += Time.deltaTime * speed * rider.speed;
                    // rider.railT = Mathf.Clamp01(rider.railT);
                    // 
                    // // Calculate the step for travel.
                    // 
                    // // The previous position of the rider.
                    // Vector3 riderOldPos = rider.transform.position;
                    // 
                    // // Update the postion.
                    // rider.transform.position =
                    //     Vector3.Lerp(
                    //         rider.startPoint.transform.position,
                    //         rider.endPoint.transform.position,
                    //         rider.railT);
                    // 

                    // Move Towards (New)
                    // Movement step.
                    float step = speed * rider.speed * Time.deltaTime;
                    
                    // Old position of the rider.
                    Vector3 riderOldPos = rider.transform.position;

                    // Move towards the new position.
                    rider.transform.position = Vector3.MoveTowards(rider.transform.position, rider.endPoint.transform.position, step);

                    // Face direction of movement. If the rider didn't move, then they continue to face the same direction.
                    if(rider.transform.position != riderOldPos)
                        rider.transform.forward = (rider.transform.position - riderOldPos).normalized;

                    // TODO: fgure out the up-direction from the rail.

                    // Zeroes out the rider's velocity.
                    if (rider.rigidbody != null)
                    {
                        rider.rigidbody.velocity = Vector3.zero;
                    }

                    
                    // Marks end of segment.
                    bool endOfSegment = false;

                    // Old (Lerp)
                    // endOfSegment = rider.railT >= 1.0F

                    // New (Move Towards)
                    endOfSegment = rider.transform.position == rider.endPoint.transform.position;

                    // End of rail segment.
                    if (endOfSegment)
                    {
                        // Gets the end index.
                        int endIndex = waypoints.IndexOf(rider.endPoint);

                        // The end index is valid.
                        if (endIndex >= 0 && endIndex < waypoints.Count)
                        {
                            // Checks if this is the last point in the list.
                            if ((endIndex + 1) < waypoints.Count) // Not last point.
                            {
                                rider.startPoint = rider.endPoint;
                                rider.endPoint = waypoints[endIndex + 1];
                                // rider.railT = 0.0F; // No longer using lerp.
                            }
                            else // Last point.
                            {
                                // If the rail loops, loop back to the start.
                                if (loopRail)
                                {
                                    rider.startPoint = rider.endPoint;
                                    rider.endPoint = waypoints[0];
                                    // rider.railT = 0.0F; // No longer using lerp.
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
                    rider.OnPositionUpdated(riderOldPos, rider.transform.position);
                }
            }
            
        }
    }
}