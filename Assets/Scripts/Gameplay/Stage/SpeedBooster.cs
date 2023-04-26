using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Boosts the player in a given direction.
    public class SpeedBooster : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        // The amount of power that's given to the speed booster. 
        public float power = 10.0F;

        // If 'true', the player's velocity is set to a fixed amount, rather than being additive.
        public bool setVelocity = true;

        // If 'true', the speed boost forward is used for the player's boost.
        // If 'false', the player's movement direction upon hitting the spood boster is used.
        public bool useSpeedBoostForward = true;

        [Header("After Effects")]

        // If 'true', the speed booster locks the player's controls for a certain period of time.
        public bool lockPlayerControls = true;

        // If set to 'true', the speed booster will disable gravity on the rigidbody as dictated by lock time.
        public bool disableGravity = false;

        // The amount of time (in seconds) that the player's controls are locked for.
        public float lockTime = 0.85F;

        // Start is called before the first frame update
        void Start()
        {
            // Grabs the instance.
            if(gameManager == null)
                gameManager = GameplayManager.Instance;
        }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider other)
        {
            // Checks the tag to know that it's a player.
            if(other.gameObject.tag == Player.PLAYER_TAG)
            {
                Player player;

                // Gets the player component.
                if(other.gameObject.TryGetComponent(out player))
                {
                    // Applies the speed boost to the player.
                    ApplySpeedBoost(player);
                }
            }
        }

        // Applies force to the provided player's rigid body.
        public void ApplySpeedBoost(Player player)
        {
            // Sets the direction of movement.
            Vector3 direc;

            // Checks if the speed booster forward should be used.
            if (useSpeedBoostForward) // Move in direction pointed by speed booster.
            {
                direc = transform.forward;
            }
            else // Move in player's direction.
            {
                // If the player isn't moving, they go in the direction they're facing.
                // If the player is moving, they go in their direction of movement.
                direc = (player.physicsBody.velocity == Vector3.zero) ? 
                    player.transform.forward : player.physicsBody.velocity.normalized;
            }

            // Zeroes out the velocity.
            if (setVelocity)
            {
                // Reset physics body.
                player.physicsBody.velocity = Vector3.zero;
                player.physicsBody.angularVelocity = Vector3.zero;

                // Reset model body.
                if(player.modelBody != null)
                {
                    player.modelBody.velocity = Vector3.zero;
                    player.modelBody.angularVelocity = Vector3.zero;
                }
            }

            // Disables the player's gravity.
            if (disableGravity)
            {
                player.physicsBody.useGravity = false;

                if(player.modelBody != null)
                    player.modelBody.useGravity = false;
            }


            // Adds force to the physics body and model body.
            switch(player.MovementMode)
            {
                case Player.MoveMode.fourWay: // Four-way directional movement.
                    // No rotation.
                    break;

                case Player.MoveMode.forwardOnly: // Forward only.
                    player.transform.forward = direc; // Has the player face the direction of movement.

                    // The player can only rotate on the y-axis.
                    player.transform.eulerAngles = Vector3.Scale(player.transform.eulerAngles, new Vector3(0.0F, 1.0F, 0.0F));
                    break;
            }


            // Add force.
            player.physicsBody.AddForce(direc * power, ForceMode.VelocityChange);

            if(player.modelBody != null)
                player.modelBody.AddForce(direc * power, ForceMode.VelocityChange);


            // Locks player input for the set time.
            if(lockPlayerControls)
                player.inputUnlockTimer = lockTime;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}