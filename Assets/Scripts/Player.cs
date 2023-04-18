using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // The script for the player.
    public class Player : MonoBehaviour
    {
        // The player's rigidbody.
        // Freeze the y-position. The y-position is manually set by the user.
        public Rigidbody rigidBody;

        // The camera following the player. This is used to determine how the player's inputs are processed.
        public FollowerCamera followerCamera;

        // The parent of the follower camera.
        private Transform followerCameraParent = null;

        // The rail rider script.
        public RailRider railRider;

        // The player's movement speed.
        private float moveSpeed = 20.0F; // TODO: make public when finished.

        // The player's rotation incrementer (in degrees).
        private float rotationInc = 60.0F; // TODO: make public when finished.

        // The player's move speed when they rotate.
        private float rotMoveSpeed = 8.0F; // TODO: make public when finished.

        // The player's jump power.
        private float jumpPower = 5.0F; // TODO: make public when finished.

        // If the player can jump.
        private bool canJump = true;

        // Position when the player jumped.
        private Vector3 posOnJump = Vector3.zero;

        // // The up vector of the player (TODO: address)
        // private Vector3 playerUp = Vector3.up;

        // Start is called before the first frame update
        void Start()
        {
            // Checks if the rigidbody exists.
            if(rigidBody == null)
            {
                // Tries to get the rigidbody.
                if(!TryGetComponent(out rigidBody))
                {
                    // Add the rigidbody.
                    rigidBody = gameObject.AddComponent<Rigidbody>();
                }
            }

            // Grabs the rail rider.
            if (railRider == null)
                railRider = gameObject.GetComponent<RailRider>();


            // Rail rider is set.
            if(railRider != null)
            {
                railRider.OnAttachToRailAddCallback(OnAttachToRail);
                railRider.OnDetachFromRailAddCallback(OnDetachFromRail);
                railRider.OnPositionUpdatedAddCallback(OnRailPositionUpdated);
            }
        }

        //// OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        //private void OnCollisionEnter(Collision collision)
        //{
        //    // If it's a rail object.
        //    if (collision.gameObject.tag == Rail.RAIL_TAG)
        //    {
        //        OnAttachToRail();
        //    }
        //}

        // OnCollisionStay is called once per frame for every collider/rigidbody that is touching this rigidbody/collider.
        private void OnCollisionStay(Collision collision)
        {
            // TODO: check tag to see if object should effect player's movement direction.

            // // Gets the up direction of the collision.
            // playerUp = collision.transform.up;

            // If it's a ground object. (TODO: check contact point so that the player is standing on the platform).
            if (collision.gameObject.tag == GameplayManager.GROUND_TAG)
            {
                canJump = true;
                EnableCameraTrackPlayerY();
            }
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            // // Note: you probably need to figure out a better way to do this.
            // playerUp = Vector3.up;

            // If it's a ground object. (TODO: check contact point so that the player is standing on the platform).
            if (collision.gameObject.tag == GameplayManager.GROUND_TAG)
            {
                canJump = false;
            }
            //else if(collision.gameObject.tag == Rail.RAIL_TAG) // Rail
            //{
            //    OnDetachFromRail();
            //}
                
        }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider collision)
        {
            //// If it's a rail object.
            //if (collision.gameObject.tag == Rail.RAIL_TAG)
            //{
            //    OnAttachToRail();
            //}
        }

        // OnTriggerExit is called when the Collider other has stopped touching the trigger.
        private void OnTriggerExit(Collider other)
        {
            //// If it's a rail object.
            //if (other.gameObject.tag == Rail.RAIL_TAG)
            //{
            //    OnDetachFromRail();
            //}
        }

        // Called when attaching to a rail.
        private void OnAttachToRail(Rail rail, RailRider rider)
        {
            canJump = true;
            EnableCameraTrackPlayerY();

            followerCameraParent = followerCamera.transform.parent;
            followerCamera.transform.parent = transform;
        }

        // Called when detaching from a rail.
        private void OnDetachFromRail(Rail rail, RailRider rider)
        {
            followerCamera.transform.parent = followerCameraParent;
        }

        // Called when changing positions on a rail.
        private void OnRailPositionUpdated(Vector3 oldPos, Vector3 newPos)
        {
            // ...
        }

        // Updates the player's inputs.
        private void UpdateInput()
        {
            // The horizontal and vertical.
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            float jump = Input.GetAxisRaw("Jump");

            // NOTE: you need to account for applying force when on slopes. Maybe have a box that's used to...
            // Define how the forces are applied, and have a sphere on the inside that actually rotates...
            // So have two rigidbodies and apply the velocities to both of them.

            // Left/Right
            if (hori != 0.0F)
            {
                // Rotation
                float rotAngle = rotationInc * hori * Time.deltaTime;
                
                // Gets the camera's old parent, and sets its parent as being the current object.
                followerCameraParent = followerCamera.transform.parent;
                followerCamera.transform.parent = transform;

                // Rotates the player.
                transform.Rotate(Vector3.up, rotAngle);

                // Sets the camera back to normal.
                followerCamera.transform.parent = followerCameraParent;

                // Calculates the new offset.
                followerCamera.posOffset = GameplayManager.RotateY(followerCamera.posOffset, rotAngle, true); // Rotation version.
                
                
                // Offset based on new positions - not using it since the camera pos may be different from its offset.
                // playerCamera.posOffset = playerCamera.transform.position - playerCamera.target.transform.position;

                // playerCamera.transform.RotateAround(transform.position, Vector3.up, rotAngle);

                // Movement
                // Vector3 direc = (playerCamera != null) ? playerCamera.transform.right : transform.right; // Original
                // rigidBody.AddForce(direc * moveSpeed * hori, ForceMode.Impulse); // Original

                Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward: transform.forward; // New
                rigidBody.AddForce(direc * rotMoveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // New
            }

            // Forward/Back
            if (vert != 0.0F)
            {
                Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward : transform.forward;
                rigidBody.AddForce(direc * moveSpeed * vert * Time.deltaTime, ForceMode.Impulse);
            }


            // Jump
            if(canJump) // Player can jump.
            {
                // If the player should jump.
                if (jump != 0.0F)
                {
                    // Saves the player's position when they jumped.
                    posOnJump = transform.position;

                    // Apply jump.
                    Vector3 direc = (followerCamera != null) ? followerCamera.transform.up : transform.up;
                    rigidBody.AddForce(direc * jumpPower * jump, ForceMode.Impulse);

                    // The player cannot jump, and their y-position shouldn't be followed.
                    canJump = false;
                    
                    // Don't follow the player.
                    followerCamera.followY = false;

                }
            }


            // If the player's y-position is currently not being followed.
            if(!followerCamera.followY)
            {
                // If the player is descending again, start following their y-position once more.
                if ((transform.position - posOnJump).y < 0)
                    followerCamera.followY = true;
            }
        }

        // Call to have the camera track the player's y-position again.
        public void EnableCameraTrackPlayerY()
        {
            followerCamera.followY = true;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInput();
        }

        // This function is called when the MonoBehaviour will be destroyed.
        private void OnDestroy()
        {
            railRider.OnAttachToRailRemoveCallback(OnAttachToRail);
            railRider.OnDetachFromRailRemoveCallback(OnDetachFromRail);
            railRider.OnPositionUpdatedRemoveCallback(OnRailPositionUpdated);
        }
    }
}