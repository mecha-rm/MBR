using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // The movement being controlled.
    public class ControlledMovement : MonoBehaviour
    {
        // The rigidbody.
        public new Rigidbody rigidbody;

        // The force mode for the rigidbody.
        public ForceMode forceMode = ForceMode.Impulse;

        // The speed of the entity.
        public float speed = 30.0F;

        // The max speed of the entity.
        public float maxSpeed = 50.0F;

        // Set to 'true' to update inputs.
        public bool updateInput = true;

        // Start is called before the first frame update
        void Start()
        {
            // Grab the rigidbody if it's not set.
            if(rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
        }

        // Updates the entity inputs.
        private void UpdateInputs()
        {
            // Calculates the movement.
            Vector3 move = Vector3.zero;
            move.x = Input.GetAxisRaw("Horizontal");
            move.z = Input.GetAxisRaw("Vertical");

            // Move the entity.
            if (move != Vector3.zero)
            {
                // Add force to the rigidbody.
                rigidbody.AddForce(move * Time.deltaTime, forceMode);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Updates the inputs of the user.
            if (updateInput)
                UpdateInputs();

            // Clamping the speed.
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
        }
    }
}