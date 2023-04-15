using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // The script for the player.
    public class Player : MonoBehaviour
    {
        // The player's rigidbody.
        public Rigidbody physicsBody;

        // The rigidbody of the model.
        public Rigidbody modelBody;

        // The player's speed.
        private float speed = 1.0F;

        // The up vector of the player
        private Vector3 playerUp = Vector3.up;

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

            // Tries to get the model body - make sure all position components are frozen.
            if(modelBody == null)
                modelBody = GetComponentInChildren<Rigidbody>();

        }

        // OnCollisionStay is called once per frame for every collider/rigidbody that is touching this rigidbody/collider.
        private void OnCollisionStay(Collision collision)
        {
            // TODO: check tag to see if object should effect player's movement direction.

            // Gets the up direction of the collision.
            playerUp = collision.transform.up;

            
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            // Note: you probably need to figure out a better way to do this.
            playerUp = Vector3.up;
        }

        // Updates the player's inputs.
        private void UpdateInput()
        {
            // The horizontal and vertical.
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");

            // NOTE: you need to account for applying force when on slopes. Maybe have a box that's used to...
            // Define how the forces are applied, and have a sphere on the inside that actually rotates...
            // So have two rigidbodies and apply the velocities to both of them.

            // Left/Right
            if (hori != 0.0F)
            {
                physicsBody.AddForce(playerUp * speed * hori, ForceMode.Impulse);
            }

            // Forward/Back
            if (vert != 0.0F)
            {
                physicsBody.AddForce(playerUp * speed * vert, ForceMode.Impulse);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInput();   
        }
    }
}