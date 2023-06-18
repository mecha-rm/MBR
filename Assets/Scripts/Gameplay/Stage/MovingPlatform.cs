using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // NOTE: make sure the object with the platform script has a scale of (1, 1, 1), otherwise it will screw up...
    // The scale of the passangers.

    // Moves a platform in a given direction.
    public abstract class MovingPlatform : MonoBehaviour
    {
        // A passenger on the platform. This is used to parent the platform so that entities move with it.
        protected struct Passenger
        {
            // The game object that's riding the platform.
            public GameObject gameObject;

            // The original parent of the game object.
            public Transform originalParent;
        }

        // The collider for the moving platform.
        public new Collider collider;

        // The list of passengers on the moving platform.
        protected List<Passenger> passengers = new List<Passenger>();

        // The valid tags for the platform. If this list is empty, then all tags are valid.
        public List<string> tags = new List<string>();

        // The destination index.
        public int destIndex = 0;

        // The speed of the movement.
        public float speed = 1.0F;

        // Reverses the movement direction if 'true'.
        public bool reversed = false;

        // Pauses movement of the platform.
        public bool paused = false;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // Grabs the collider if it's not set.
            if (collider == null)
                collider = GetComponent<Collider>();
        }

        // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        private void OnCollisionEnter(Collision collision)
        {
            // Add a passenger if the tag is valid.
            if (tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                AddPassenger(collision.gameObject);
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            // Remove a passenger if the tag is valid.
            RemovePassenger(collision.gameObject);
        }

        // OnTriggerEnter is called when the Collider enters another the trigger.
        private void OnTriggerEnter(Collider other)
        {
            // Add a passenger if the tag is valid.
            if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                AddPassenger(other.gameObject);
        }

        // OnTriggerExit is called when the Collider other has stopped touching the trigger.
        private void OnTriggerExit(Collider other)
        {
            // Tries to remove a passanger.
            RemovePassenger(other.gameObject);
        }

        // Gets the waypoint.
        public abstract Vector3 GetWaypoint(int index);

        // Gets the waypoint count.
        public abstract int GetWaypointCount();


        // Checks if the entity is a passanger.
        public bool HasPassanger(GameObject entity)
        {
            // Result of check.
            bool result = false;

            // Goes through each passenger.
            foreach(Passenger passenger in passengers)
            {
                // Checks if the game object is equal to the entity.
                if(passenger.gameObject == entity)
                {
                    // Passenger found.
                    result = true;
                    break;
                }
            }

            return result;
        }

        // Adds a passanger.
        public bool AddPassenger(GameObject entity)
        {
            // Checks if the passanger is in the list.
            bool inList = HasPassanger(entity);

            if(inList) // Already in list, so do nothing.
            {
                return false;
            }
            else // Not in list, so add it.
            {
                // TODO: for some reason, rotating causes the scale of the object to screw up when its parented.
                // You need to fix that.

                // Creates the passenger object and gives it values.
                Passenger passenger = new Passenger();
                passenger.gameObject = entity;
                passenger.originalParent = entity.transform.parent;

                // Make parent of platform (scale is adjusted due to a glitch).
                passenger.gameObject.transform.parent = transform;

                // Adds thep passenger to the list.
                passengers.Add(passenger);

                return true;
            }
        }

        // Removes a passenger.
        public bool RemovePassenger(GameObject entity)
        {
            // Checks if the passanger is in the list.
            int index = GetPassengerIndex(entity);

            if (index >= 0) // In the list, so remove them.
            {
                // Give original parent.
                if(passengers[index].gameObject != null)
                {
                    passengers[index].gameObject.transform.parent = passengers[index].originalParent;
                }
                    

                // Remove the elemnent.
                passengers.RemoveAt(index);

                // Success.
                return true;
            }
            else // Not in list, so do nothing.
            {
                return false;
            }
        }

        // Gets the index of the provided passenger. Returns -1 if they're not in the list.
        public int GetPassengerIndex(GameObject entity)
        {
            // Passenger index.
            int index = -1;

            // Goes through all passengers.
            for(int i = 0; i < passengers.Count; i++)
            {
                // Found the entity.
                if (passengers[i].gameObject == entity)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        // Clears out all passengers.
        public void ClearPassengers()
        {
            // Clears out all passengers.
            foreach(Passenger passenger in passengers)
            {
                // Give the passenger its original parent.
                if (passenger.gameObject != null)
                    passenger.gameObject.transform.parent = passenger.originalParent;
            }

            // Clear the list.
            passengers.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            // Platform should be moving.
            if(!paused)
            {
                // Gets the destination waypoint.
                Vector3 destination = GetWaypoint(destIndex);

                // Movement (TODO: make spline version?).
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, destination, step);

                // Destination reached.
                if(transform.position == destination)
                {
                    // Change the index.
                    destIndex += (reversed) ? -1 : 1;

                    // Change the destination index.
                    if (destIndex < 0) // Reversed
                    {
                        destIndex = GetWaypointCount() - 1;
                    }
                    else if (destIndex >= GetWaypointCount()) // Forward
                    {
                        destIndex = 0;
                    }
                }
            }
        }
    }
}