using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace mbs
{
    // Attach this to an object if you want it to be effected by the death plane.
    public class DeathPlaneTrigger : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        // If set to 'true', the object gets destroyed when it touches the death plane.
        public bool destroyOnTriggered = false;

        // Start is called before the first frame update
        void Start()
        {
            // Grabs the death plane instance.
            if (gameManager == null)
                gameManager = GameplayManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            // If the element is below the death plane.
            if(transform.position.y <= gameManager.deathPlaneY)
            {
                // Call the gameplay manager with the object.
                gameManager.OnDeathPlaneReached(gameObject);

                // Destroys the game object.
                if(destroyOnTriggered)
                    Destroy(gameObject);

            }
        }
    }
}