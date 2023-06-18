using Cinemachine;
using Newtonsoft.Json.Linq;
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

        // TODO: add mode for going four-way relative to the camera position.
        // Movement modes
        /*
         * fourWay: full directional movement relative to the world transform.
         * fourWayCam: full directional movement relative to the camera target.
         * forwardOnly: forward and back movement with rotation on horizontal.
         * xy2dWorld: move on a 2D plane (XY) relative to the world axis. Make sure to disable the unused axis.
         * xy2dCam: move on a 2D plane (XY) relative to the camera target. Make sure to disable the unused axis.
         * zy2dWorld: move on a 2D plane (ZY) relative to the world axis. Make sure to disable the unused axis.
         * zy2dCam: move on a 2D plane (ZY) relative to the camera target. Make sure to disable the unused axis.
         */
        public enum MovementMode { fourWayWorld, fourWayCam, forwardOnly, xy2dWorld, xy2dCam, zy2dWorld, zy2dCam }

        // The player's movement mode - this is used for testing purposes, and should likely be removed later.
        private MovementMode moveMode = MovementMode.fourWayCam;

        // The player's rigidbody.
        // Freeze the y-position. The y-position is manually set by the user.
        // Make sure to freeze the rotation, but leave the position.
        public new Rigidbody rigidbody;

        // The target for virtual cameras.
        // NOTE: if you use this for the virtual camera, make sure damping is set 0 for all pos and rotation.
        // It causes stuttering if you don't.
        public TransformCopy cameraTarget;

        // The rail rider script.
        public RailRider railRider;


        // The limit on the player's velocity.
        public float maxVelocity = 1000.0F;

        [Header("Input")]

        // Enables/disables user inputs.
        public bool inputsEnabled = true;

        // The player's movement speed.
        private float moveSpeed = 30.0F; // TODO: make public when finished.

        // The player's rotation incrementer (in degrees).
        private float rotationInc = 60.0F; // TODO: make public when finished.

        // The player's move speed when they rotate.
        // private float rotMoveSpeed = 8.0F; // TODO: make public when finished.

        [Header("Input/Jump")]

        // The jump key.
        public KeyCode jumpKey = KeyCode.Space;

        // The player's jump power.
        public float jumpPower = 20.0F; // TODO: make public when finished.

        // If the player can jump.
        private bool canJump = true;

        // Position when the player jumped.
        private Vector3 posOnJump = Vector3.zero;

        // // The up vector of the player (TODO: address)
        // private Vector3 playerUp = Vector3.up;

        [Header("Input/Dash")]

        // The dash key.
        public KeyCode dashKey = KeyCode.H;

        // The power of the player's dash.
        public float dashPower = 10.0F;

        // The dash cooldown timer.
        public float dashCooldownTimer = 0.0F;

        // The dash cooldown timer's max.
        public float dashCooldownTimerMax = 1.5F;


        [Header("Input/Other")]

        // Unlocks movement for the player after this timer is set to 0 (time in seconds). This does NOT use inputEnabled.
        // This timer won't go down if inputsEnabled is false.
        public float inputUnlockTimer = 0.0F;

        // Start is called before the first frame update
        void Start()
        {
            // Checks if the rigidbody exists.
            if(rigidbody == null)
            {
                // Tries to get the rigidbody.
                if(!TryGetComponent(out rigidbody))
                {
                    // Add the rigidbody.
                    rigidbody = gameObject.AddComponent<Rigidbody>();
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

            // Update the movement mode.
            SetMovementMode(moveMode);
        }

        //// OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        //private void OnCollisionEnter(Collision collision)
        //{
        //    // // If it's a rail object.
        //    // if (collision.gameObject.tag == Rail.RAIL_TAG)
        //    // {
        //    //     OnAttachToRail();
        //    // }

        //    // If it's a ground object. (TODO: check contact point so that the player is standing on the platform).
        //    // This is used in 'Enter' so that it doesn't hinder the player's jump.
        //    if (collision.gameObject.tag == GameplayManager.GROUND_TAG)
        //    {
        //        canJump = true;
        //        SetCameraTrackPlayerY(true);
        //    }
        //}

        // TODO: account for hitting the side of a ground object. Have other non-tagged collider.

        // OnCollisionStay is called once per frame for every collider/rigidbody that is touching this rigidbody/collider.
        private void OnCollisionStay(Collision collision)
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

        // Gets the movement mode.
        public MovementMode GetMovementMode()
        {
            return moveMode;
        }


        // Sets the movement mode.
        public void SetMovementMode(MovementMode newMode)
        {
            // Sets the new movement mode.
            moveMode = newMode;

            // Sets rigidbody constraints based on the mode being set.

            // Removing this since the ball should rotate properly.
            // Checks if the new move mode is 2D or 3D.
            switch (moveMode)
            {
                // 3D movement.
                case MovementMode.fourWayWorld:
                case MovementMode.fourWayCam:
                case MovementMode.forwardOnly:

                    // Allow all axes of movement.
                    rigidbody.constraints = RigidbodyConstraints.None;
                    // rigidbody.constraints = RigidbodyConstraints.FreezeRotation; // No longer needed.
                    break;

                // 2d movement (world and cam) - XY
                case MovementMode.xy2dWorld: 
                case MovementMode.xy2dCam:
                    // Frozen on z-axis position (no longer freezes on rotation).

                    rigidbody.constraints = RigidbodyConstraints.None;
                    // rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation; // Old
                    rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
                    break;

                // 2d movement 9world and cam) - ZY
                case MovementMode.zy2dWorld:
                case MovementMode.zy2dCam:
                    // frozen on x-axis (no longer freezes on rotation).

                    rigidbody.constraints = RigidbodyConstraints.None;
                    // rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation; // Old
                    rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
                    break;

            }
        }


        // Called when attaching to a rail.
        private void OnAttachToRail(Rail rail, RailRider rider)
        {
            canJump = true;
            SetCameraTrackPlayerY(true);
        }

        // Called when detaching from a rail.
        private void OnDetachFromRail(Rail rail, RailRider rider)
        {
            // ...
        }

        // Called when changing positions on a rail.
        private void OnRailPositionUpdated(Vector3 oldPos, Vector3 newPos)
        {
            // ...
        }


        // INPUTS //

        // Updates the player's inputs.
        private void UpdateInput()
        {
            // The horizontal and vertical.
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");

            // Need to set it up this way for GetKeyDown for jumping.
            // float jump = Input.GetAxisRaw("Jump"); // Old
            float jump = Input.GetKeyDown(jumpKey) ? 1.0F : 0.0F; // New

            // NOTE: you need to account for applying force when on slopes. Maybe have a box that's used to...
            // Define how the forces are applied, and have a sphere on the inside that actually rotates...
            // So have two rigidbodies and apply the velocities to both of them.

            // Left/Right
            if (hori != 0.0F)
            {
                // The movement direction.
                Vector3 direc = Vector3.zero;

                switch (moveMode)
                {
                    case MovementMode.fourWayWorld: // Four-way movement (world)

                        // Movement
                        // Vector3 direc = (playerCamera != null) ? playerCamera.transform.right : transform.right; // Original
                        direc = Vector3.right;
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Original

                        // Apply force in the direction.
                        // Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward: transform.forward; // New
                        // rigidBody.AddForce(direc * rotMoveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // New
                        break;


                    case MovementMode.fourWayCam: // Four-way movement (camera)
                        // Movement
                        direc = cameraTarget.transform.right;
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Original

                        break;

                    case MovementMode.forwardOnly: // Forward movement only.
                        // Rotation
                        float rotAngle = rotationInc * hori * Time.deltaTime;

                        // Old - rotates the player.
                        // transform.Rotate(Vector3.up, rotAngle);

                        // New - rotates the camera target.
                        cameraTarget.transform.Rotate(Vector3.up, rotAngle);

                        break;

                    case MovementMode.xy2dWorld: // 2D Movement (XY) - World
                        
                        direc = Vector3.right;
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Move on the x-axis.

                        break;

                    case MovementMode.xy2dCam: // 2D Movement (XY) - Camera Target

                        direc = cameraTarget.transform.right;
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Move on the x-axis.

                        break;

                    case MovementMode.zy2dWorld: // 2D Movement (ZY) - World
                        direc = Vector3.forward; // Forward
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Move on the z-axis.

                        break;

                    case MovementMode.zy2dCam: // 2D Movement (ZY) - Camera Target
                        direc = cameraTarget.transform.forward; // Forward
                        rigidbody.AddForce(direc * moveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // Move on the z-axis.

                        break;

                }
                


            }

            // Forward/Back (3D only)
            if (vert != 0.0F)
            {
                // The travel direction.
                Vector3 direc = Vector3.zero;

                // Changes how direction is calculated.
                switch (moveMode)
                {
                    case MovementMode.fourWayWorld: // All directional movement (camera).
                        direc = Vector3.forward;
                        break;

                    case MovementMode.fourWayCam: // All directional movement (camera).
                    case MovementMode.forwardOnly: // Forward movement only.
                        direc = cameraTarget.transform.forward;
                        break;
                }
                
                // Applies Force
                switch (moveMode)
                {
                    case MovementMode.fourWayWorld: // All directional movement.
                    case MovementMode.fourWayCam:
                    case MovementMode.forwardOnly: // Forward movement only.
                        // Old
                        // Vector3 direc = (followerCamera != null) ? followerCamera.transform.forward : transform.forward;

                        // New - Ver. 1 (make sure the object's forward is not changed).
                        // Vector3 direc = transform.forward; // Old
                        // Vector3 direc = Vector3.forward; // New

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
                        rigidbody.AddForce(force, ForceMode.Impulse);

                        break;
                }
            }

            // Jump
            // if (canJump && Input.GetKeyDown(KeyCode.Space))
            if (canJump && Input.GetKeyDown(jumpKey)) // Player can jump.
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
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0.0F, rigidbody.velocity.z);

                    // Applies the forces.
                    Vector3 force = direc.normalized * jumpPower * jump;
                    rigidbody.AddForce(force, ForceMode.Impulse); // Original

                    // The player cannot jump, and their y-position shouldn't be followed.
                    canJump = false;
                    
                    // Don't follow the player.
                    SetCameraTrackPlayerY(false);
                }
            }


            // Dash
            // Checks if the player is allowed to dash.
            if(dashCooldownTimer <= 0.0F)
            {
                // The dash key.
                if(Input.GetKeyDown(dashKey))
                {
                    // TODO: you should rewrite this.

                    // The dash direction.
                    Vector3 direc = Vector3.zero;

                    // Horizontal input is set.
                    if (hori != 0.0F)
                    {
                        // Right and Forward
                        Vector3 right = Vector3.right;
                        Vector3 forward = Vector3.forward;

                        // Sets the axis direction based on movement mode.
                        switch (moveMode)
                        {
                            case MovementMode.fourWayWorld: // ALL Directional (World)
                                right = Vector3.right;
                                break;

                            case MovementMode.fourWayCam: // All Directional (Cam)
                                right = cameraTarget.transform.right;
                                break;

                            case MovementMode.xy2dWorld: // X-Forward (World)
                                right = Vector3.right;
                                break;

                            case MovementMode.xy2dCam: // X-Forward (Cam)
                                forward = cameraTarget.transform.right;
                                break;

                            case MovementMode.zy2dWorld: // Z-Forward (World)
                                forward = Vector3.forward;
                                break;

                            case MovementMode.zy2dCam: // Z-Forward (Cam)
                                forward = cameraTarget.transform.forward;
                                break;
                        }

                        // Add horizontal movement.
                        switch (moveMode)
                        {
                            case MovementMode.fourWayWorld: // Four-directional (World)
                            case MovementMode.fourWayCam: // Four-directional (Cam)
                            case MovementMode.xy2dWorld: // X-Forward (World)
                            case MovementMode.xy2dCam: // X-Forward (Cam)
                                direc += right * (hori >= 0.0F ? 1.0F : -1.0F);
                                break;

                            case MovementMode.zy2dWorld: // Z-Forward (World)
                            case MovementMode.zy2dCam: // Z-Forward (Cam)
                                direc += forward * (hori >= 0.0F ? 1.0F : -1.0F);
                                break;
                        }
                    }

                    // Vertical input is set.
                    if(vert != 0.0F)
                    {
                        Vector3 forward = Vector3.forward;

                        // Add vertical movement.
                        switch (moveMode)
                        {
                            case MovementMode.fourWayWorld: // Four-directional (based on world).
                                forward = Vector3.forward;
                                break;

                            case MovementMode.fourWayCam: // Four-directional (based on camera).
                            case MovementMode.forwardOnly: // Forward only.
                                forward = cameraTarget.transform.forward;
                                break;
                        }

                        // Add vertical movement.
                        switch (moveMode)
                        {
                            case MovementMode.fourWayWorld: // Four-directional (based on world).
                            case MovementMode.fourWayCam: // Four-directional (based on camera).
                            case MovementMode.forwardOnly: // Forward only.
                                direc += forward * (vert >= 0.0F ? 1.0F : -1.0F);
                                break;
                        }
                    }



                    // If the direction is 0, then go in the direction the player is already moving.
                    if (direc == Vector3.zero)
                    {
                        direc = rigidbody.velocity.normalized;
                    }
                        

                    // Apply force to hte physics body.
                    rigidbody.AddForce(direc.normalized * dashPower, ForceMode.Impulse);

                    // Return timer to max.
                    dashCooldownTimer = dashCooldownTimerMax;
                }
            }
            else
            {
                // Reduce timer.
                dashCooldownTimer -= Time.deltaTime;

                // Bounds.
                if (dashCooldownTimer <= 0)
                    dashCooldownTimer = 0.0F;
            }


            // If the player's y-position is currently not being followed.
            if(!cameraTarget.copyPositionY)
            {
                // If the player is descending again, start following their y-position once more.
                // It also only happens if the rigidbody's velocity is negative or 0 (TODO: may not check velocity).
                if ((transform.position - posOnJump).y < 0 && rigidbody.velocity.y < 0)
                {
                    SetCameraTrackPlayerY(true);
                }
                    
            }
                
        }

        // Call to have the camera track the player's y-position again.
        public void SetCameraTrackPlayerY(bool value)
        {
            cameraTarget.copyPositionY = value;
        }

        // Update is called once per frame
        void Update()
        {
            // Updates player's inputs.
            if(inputsEnabled)
            {
                // Checks if the player is allowed to have control.
                if(inputUnlockTimer <= 0.0F)
                {
                    UpdateInput();
                }
                else // Reduce timer.
                {
                    // Reduces the timer.
                    inputUnlockTimer -= Time.deltaTime;

                    // Makes sure it stops at 0.
                    if (inputUnlockTimer <= 0)
                    {
                        // Timer set to 0.
                        inputUnlockTimer = 0.0F;

                        // Turn gravity back on.
                        rigidbody.useGravity = true;
                    }
                        
                }
            }

            // Clamps the velocity of the physics body.
            if (rigidbody.velocity.magnitude > Mathf.Abs(maxVelocity))
                rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxVelocity);
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