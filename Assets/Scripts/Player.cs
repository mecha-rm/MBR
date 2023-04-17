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
        public FollowerCamera playerCamera;

        // The player's movement speed.
        private float moveSpeed = 20.0F;

        // The player's rotation incrementer (in degrees).
        private float rotationInc = 60.0F;

        // The player's move speed when they rotate.
        private float rotMoveSpeed = 8.0F;

        // The player's jump power.
        private float jumpPower = 1.0F;



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

        }

        // OnCollisionStay is called once per frame for every collider/rigidbody that is touching this rigidbody/collider.
        private void OnCollisionStay(Collision collision)
        {
            // TODO: check tag to see if object should effect player's movement direction.

            // // Gets the up direction of the collision.
            // playerUp = collision.transform.up;

            
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            // // Note: you probably need to figure out a better way to do this.
            // playerUp = Vector3.up;
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
                Transform oldCamParent = playerCamera.transform.parent;
                playerCamera.transform.parent = transform;

                // Rotates the player.
                transform.Rotate(Vector3.up, rotAngle);

                // Sets the camera back to normal.
                playerCamera.transform.parent = oldCamParent;

                // Calculates the new offset.
                playerCamera.posOffset = GameplayManager.RotateY(playerCamera.posOffset, rotAngle, true); // Rotation version.
                
                
                // Offset based on new positions - not using it since the camera pos may be different from its offset.
                // playerCamera.posOffset = playerCamera.transform.position - playerCamera.target.transform.position;

                // playerCamera.transform.RotateAround(transform.position, Vector3.up, rotAngle);

                // Movement
                // Vector3 direc = (playerCamera != null) ? playerCamera.transform.right : transform.right; // Original
                // rigidBody.AddForce(direc * moveSpeed * hori, ForceMode.Impulse); // Original

                Vector3 direc = (playerCamera != null) ? playerCamera.transform.forward: transform.forward; // New
                rigidBody.AddForce(direc * rotMoveSpeed * hori * Time.deltaTime, ForceMode.Impulse); // New
            }

            // Forward/Back
            if (vert != 0.0F)
            {
                Vector3 direc = (playerCamera != null) ? playerCamera.transform.forward : transform.forward;
                rigidBody.AddForce(direc * moveSpeed * vert * Time.deltaTime, ForceMode.Impulse);
            }


            // Jump
            if(jump != 0.0F)
            {
                // TODO: make it so that the player cannot jump in the air.
                Vector3 direc = (playerCamera != null) ? playerCamera.transform.up : transform.up;
                rigidBody.AddForce(direc * jumpPower * jump, ForceMode.Impulse);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInput();   
        }
    }
}