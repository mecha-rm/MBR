using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace mbs
{
    // Translates an object between several points.
    public abstract class Translation : MonoBehaviour
    {
        // The rigidbody of the object. 
        // Recommended that you disable gravity.
        public new Rigidbody rigidbody;

        // Determines if the rigidbody should be used for movement or not.
        // It's recommended that the rigidbody transforms be disabled if it's not being used.
        public bool useRigidbody = true;

        // The speed the object moves at.
        public float speed = 5.0F;

        // The start index of the object.
        public int destIndex = 0;

        // // If set to 'true', the velocity is set to 0 when a waypoint is reached. - Not needed.
        // [Tooltip("Zeroes out the velocity when the position is reached.")]
        // public bool stopAtDest = false;

        // The position on the prior frame.
        private Vector3 oldPos;

        // Pauses the object if true.
        public bool paused = false;

        // Start is called before the first frame update
        void Start()
        {
            // Gets the rigidbody attached to the object.
            if(rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();

            // The object's starting position.
            oldPos = transform.position;
        }

        // Gets the total number of waypoints.
        public abstract int GetWaypointCount();

        public abstract Vector3 GetWaypoint(int index);

        // Update is called once per frame
        void Update()
        {
            // TODO: the calculation is bad. Fix that.
            if(!paused)
            {
                // Checks if the rigidbody should be used.
                if(useRigidbody)
                {
                    // If the destination has been reached, increase the destination index.
                    // Because the position doesn't update from the rigidbody until the next frame, it has to be done this way.
                    if (Vector3.Distance(oldPos, transform.position) >= Vector3.Distance(oldPos, GetWaypoint(destIndex)))
                    {
                        // Cancel out the velocity (this causes the objecto to stop briefly).
                        rigidbody.velocity = Vector3.zero;

                        // Set to waypoint position.
                        transform.position = GetWaypoint(destIndex);

                        // Increase the destination index.
                        destIndex++;

                        // Increment destination.
                        if (destIndex >= GetWaypointCount())
                            destIndex = 0;
                    }

                    // Gets the destination.
                    Vector3 dest = GetWaypoint(destIndex);

                    // Calculates the destination of travel.
                    Vector3 direc = dest - transform.position;
                    direc.Normalize();

                    // Applies force.
                    Vector3 force = direc.normalized * speed * Time.deltaTime;
                    rigidbody.AddForce(force, ForceMode.VelocityChange);


                    // Copies the transform position.
                    oldPos = transform.position;
                }
                else
                {
                    // Gets the destination.
                    Vector3 dest = GetWaypoint(destIndex);

                    // Copies the transform position (not necessary in this movement mode).
                    oldPos = transform.position;

                    // Moves towards the destination.
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, dest, step);

                    // Checks if the object has reached its destination.
                    if(transform.position == dest)
                    {
                        // Increment destination.
                        destIndex++;

                        // Set index to 0.
                        if (destIndex >= GetWaypointCount())
                            destIndex = 0;
                    }
                }
            }
        }
    }
}