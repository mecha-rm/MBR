using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // The script for the player.
    public class Player : MonoBehaviour
    {
        // The player tag.
        public static string PLAYER_TAG = "Player";

        // The player's rigidbody.
        // Freeze the y-position. The y-position is manually set by the user.
        // Make sure to freeze the rotation, but leave the position.
        public Rigidbody physicsBody;

        // The rigidbody of the model, which is used to simulate rotations without effecting player input.
        // NOTE: make sure to freeze the position, but leave the rotation.
        public Rigidbody modelBody;

        // The camera following the player. This is used to determine how the player's inputs are processed.
        public FollowerCamera followerCamera;

        // The parent of the follower camera.
        private Transform followerCameraParent = null;

        // If set to 'true', the camera is rotated when the player rotates their body.
        public bool rotateCameraWithPlayer = true;

        // The rail rider script.
        public RailRider railRider;

        // The player's movement speed.
        private float moveSpeed = 20.0F; // TODO: make public when finished.

        // The player's rotation incrementer (in degrees).
        private float rotationInc = 60.0F; // TODO: make public when finished.

        // The player's move speed when they rotate.
        private float rotMoveSpeed = 8.0F; // TODO: make public when finished.

        // The player's jump power.
        public float jumpPower = 10.0F; // TODO: make public when finished.

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
            if(physicsBody == null)
            {
                // Tries to get the rigidbody.
                if(!TryGetComponent(out physicsBody))
                {
                    // Add the rigidbody.
                    physicsBody = gameObject.AddComponent<Rigidbody>();
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

        // OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        private void OnCollisionEnter(Collision collision)
        {
            // // If it's a rail object.
            // if (collision.gameObject.tag == Rail.RAIL_TAG)
            // {
            //     OnAttachToRail();
            // }

            // If it's a ground object. (TODO: check contact point so that the player is standing on the platform).
            // This is used in 'Enter' so that it doesn't hinder the player's jump.
            if (collision.gameObject.tag == GameplayManager.GROUND_TAG)
            {
                canJump = true;
                SetCameraTrackPlayerY(true);
            }
        }

        // // OnCollisionStay is called once per frame for every collider/rigidbody that is touching this rigidbody/collider.
        // private void OnCollisionStay(Collision collision)
        // {
        //     // TODO: check tag to see if object should effect player's movement direction.
        // 
        //     // // Gets the up direction of the collision.
        //     // playerUp = collision.transform.up;
        // 
        //     // If it's a ground object. (TODO: check contact point so that the player is standing on the platform).
        //     if (collision.gameObject.tag == GameplayManager.GROUND_TAG)
        //     {
        //         canJump = true;
        //         EnableCameraTrackPlayerY();
        //     }
        // }

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
            SetCameraTrackPlayerY(true);

            // If the camera should rotate along with the player.
            if(rotateCameraWithPlayer)
            {
                followerCameraParent = followerCamera.transform.parent;
                followerCamera.transform.parent = transform;
                followerCamera.enabled = false; // Disable script so that the camera stays in a fixed position.
            }
        }

        // Called when detaching from a rail.
        private void OnDetachFromRail(Rail rail, RailRider rider)
        {
            // If the camera should roate with the player.
            if(rotateCameraWithPlayer)
            {
                followerCamera.transform.parent = followerCameraParent;
                followerCamera.enabled = true; // Enable script so that the camera goes back to its proper position.
            }
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

            // Need to set it up this way for GetKeyDown for jumping.
            // float jump = Input.GetAxisRaw("Jump"); // Old
            float jump = Input.GetKeyDown(KeyCode.Space) ? 1.0F : 0.0F; // New

            // NOTE: you need to account for applying force when on slopes. Maybe have a box that's used to...
            // Define how the forces are applied, and have a sphere on the inside that actually rotates...
            // So have two rigidbodies and apply the velocities to both of them.

            // Left/Right
            if (hori != 0.0F)
            {
                // Rotation
                float rotAngle = rotationInc * hori * Time.deltaTime;
                
                // Gets the camera's old parent, and sets its parent as being the current object.
                if(rotateCameraWithPlayer && followerCamera.enabled)
                {
                    followerCameraParent = followerCamera.transform.parent;
                    followerCamera.transform.parent = transform;
                }

                // Rotates the player.
                transform.Rotate(Vector3.up, rotAngle);

                // Rotates the camera with the player.
                if(rotateCameraWithPlayer && followerCamera.enabled)
                {
                    // Sets the camera back to normal.
                    followerCamera.transform.parent = followerCameraParent;

                    // Calculates the new offset.
                    followerCamera.posOffset = GameplayManager.RotateY(followerCamera.posOffset, rotAngle, true); // Rotation version.


                    // Offset based on new positions - not using it since the camera pos may be different from its offset.
                    // playerCamera.posOffset = playerCamera.transform.position - playerCamera.target.transform.position;

                    // playerCamera.transform.RotateAround(transform.position, Vector3.up, rotAngle);
                }


                // Movement
                // Vector3 direc = (playerCamera != null) ? playerCamera.transform.right : transform.right; // Original
                // rigidBody.AddForce(direc * moveSpeed * hori, ForceMode.Impulse); // Original

                // Apply force in the direction.
                // Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward: transform.forward; // New
                // rigidBody.AddForce(direc * rotMoveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // New
            }

            // Forward/Back
            if (vert != 0.0F)
            {
                // Old
                // Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward : transform.forward;

                // New - Ver. 1 (make sure the object's forward is not changed).
                Vector3 direc = transform.forward;

                // New Ver. 2 - Doesn't Work
                // // The current x-orientation (needs to be reset to default for object forward).
                // float oldEulerX = transform.eulerAngles.x;
                // 
                // // Takes the euler angles and resets the x-orientation.
                // Vector3 eulers = transform.eulerAngles;
                // eulers.x = 0.0F;
                // transform.eulerAngles = eulers;
                // 
                // // Gets the forward direction.
                // Vector3 direc = transform.forward;
                // 
                // // Returns the euler back to normal.
                // eulers.x = oldEulerX;
                // transform.eulerAngles = eulers;

                // Applies force.
                Vector3 force = direc * moveSpeed * vert * Time.deltaTime;
                physicsBody.AddForce(force, ForceMode.Impulse);

                // Applies same force to the model body.
                if(modelBody != null)
                    modelBody.AddForce(force, ForceMode.Impulse);
            }


            // Jump
            // if (canJump && Input.GetKeyDown(KeyCode.Space))
            if (canJump && Input.GetKeyDown(KeyCode.Space)) // Player can jump.
            {
                // If the player should jump.
                if (jump != 0.0F)
                {
                    // Detaches the rider from the rail if it's attached to it.
                    if (railRider.IsRiderAttachedToRail())
                        railRider.DetachFromRail();

                        // Saves the player's position when they jumped.
                        posOnJump = transform.position;

                    // Apply jump.
                    // Vector3 direc = (followerCamera != null) ? followerCamera.transform.up : transform.up; // Original
                    Vector3 direc = Vector3.up; // New

                    // transform.Translate(direc.normalized);
                    physicsBody.velocity = new Vector3(physicsBody.velocity.x, 0.0F, physicsBody.velocity.z);

                    // Applies the forces.
                    Vector3 force = direc.normalized * jumpPower * jump;
                    physicsBody.AddForce(force, ForceMode.Impulse); // Original

                    // Add the impulse force.
                    if (modelBody != null)
                        modelBody.AddForce(force, ForceMode.Impulse);

                    // The player cannot jump, and their y-position shouldn't be followed.
                    canJump = false;
                    
                    // Don't follow the player.
                    SetCameraTrackPlayerY(false);
                }
            }


            // If the player's y-position is currently not being followed.
            if(!followerCamera.followY)
            {
                // If the player is descending again, start following their y-position once more.
                // It also only happens if the rigidbody's velocity is negative or 0 (TODO: may not check velocity).
                if ((transform.position - posOnJump).y < 0 && physicsBody.velocity.y < 0)
                {
                    SetCameraTrackPlayerY(true);
                }
                    
            }
        }

        // Call to have the camera track the player's y-position again.
        public void SetCameraTrackPlayerY(bool value)
        {
            followerCamera.followY = value;
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