using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // A portal that takes the valid tags to another location.
    public class Portal : MonoBehaviour
    {
        // TODO: maybe it'd be best to temporarily disable the teleport's collider...
        // As part of an animation. Work that out later.
        // The portal's collider.
        public new Collider collider;

        // The list of valid tags for the portal.
        public List<string> tags = new List<string>();

        // TODO: set variable for facing camera direction.

        // If 'true', the portal stops the velocity of the entity's rigidbody if they have one.
        public bool stopVelocity = true;

        [Header("Destination")]

        // Determines if an end portal is used or not.
        public bool useEndPoint = true;

        // The destination position. If 'useEndPoint' is false, endPoint is used instead of the set destination.
        // This is used if no endpoint is set.
        [Tooltip("The destination position. This is used if 'useEndPoint' is false.")]
        public Vector3 destination;

        // The end point.
        [Tooltip("The destination end point.")]
        public GameObject endPoint;

        // The offset from the end point.
        [Tooltip("The offset from the endpoint.")]
        public Vector3 endPointOffset;

        // The destination virtual camera. The switch only happens if the teleported entity is a player.
        // If this is null, then the camera stays the same.
        [Tooltip("The destination point's camera. If this is null, the camera isn't changed.")]
        public CinemachineVirtualCamera destVcam;

        // Start is called before the first frame update
        void Start()
        {
            // Tries to get a collider from the portal.
            if(collider == null)
                collider = GetComponent<Collider>();

        }


        // OnCollisionEnter
        private void OnCollisionEnter(Collision collision)
        {
            // Teleports the entity.
            if (tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                Teleport(collision.gameObject);
        }

        // OnTriggerEnter
        private void OnTriggerEnter(Collider other)
        {
            // Teleports the entity.
            if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                Teleport(other.gameObject);
        }


        // OnCollisionExit
        private void OnCollisionExit(Collision collision)
        {
            // // If the collider is set...
            // if(collider != null)
            // {
            //     // Enables the collider (used so that the entity isn't locked into the portal).
            //     if (!collider.enabled)
            //         collider.enabled = true;
            // }
        }

        // OnTriggerExit
        private void OnTriggerExit(Collider other)
        {
            // // If the collider is set...
            // if (collider != null)
            // {
            //     // Enables the collider (used so that the entity isn't locked into the portal).
            //     if (!collider.enabled)
            //         collider.enabled = true;
            // }
        }

        // COLLIDER //

        // Set whether or not the collider is enabled.
        public void SetColliderEnabled(bool value)
        {
            if (collider != null)
                collider.enabled = value;
        }

        // Enable the collider.
        public void EnableCollider()
        {
            SetColliderEnabled(true);
        }

        // Disable the collider.
        public void DisableCollider()
        {
            SetColliderEnabled(false);
        }

        // Toggles the collider.
        public void ToggleColliderEnabled()
        {
            if(collider != null)
                collider.enabled = !collider.enabled;
        }


        // TRANSPORTATION //

        // Returns the destination position.
        public Vector3 GetDestinationPosition()
        {
            // The position to be returned.
            Vector3 returnPos;

            // Checks if an end point should be used as the end position.
            if(useEndPoint && endPoint != null)
            {
                returnPos = endPoint.transform.position + endPointOffset;
            }
            else
            {
                // Gives the entity the destination
                returnPos = destination;
            }

            // Returns the position.
            return returnPos;
        }

        // Teleports the provided entity.
        public void Teleport(GameObject entity)
        {
            // Gets the destination position.
            Vector3 dest = GetDestinationPosition();

            // TODO: play animation.

            // Checks if the destination should change the virtual camera.
            if(destVcam != null)
            {
                // Checks if the entity is a player.
                Player player;

                // Tries to get the player component.
                if (entity.TryGetComponent(out player))
                {
                    // Grabs the gameplay manager.
                    GameplayManager.Instance.SetVirtualCamera(destVcam);
                }
            }

            // Move the entity to the destination position.
            entity.transform.position = dest;

            // Checks of the entity's velocity should be stopped.
            if (stopVelocity)
            {
                Rigidbody rigidbody;

                // Tries to get the component.
                if (entity.TryGetComponent(out rigidbody))
                {
                    // FIXME: for some reason, this doesn't appear to be work for the player.
                    // Fix.
                    rigidbody.velocity = Vector3.zero;
                }
            }
        }

    }
}