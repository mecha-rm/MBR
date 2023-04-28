using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // The game goal.
    public class Goal : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        private void OnCollisionEnter(Collision collision)
        {
            // If the player has hit the finish area.
            if(collision.gameObject.tag == Player.PLAYER_TAG)
            {
                GameplayManager.Instance.OnFinish();
            }
        }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider other)
        {
            // If the player has hit the finish area.
            if (other.gameObject.tag == Player.PLAYER_TAG)
            {
                GameplayManager.Instance.OnFinish();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}