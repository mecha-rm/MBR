using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // The stage script.
    public class Stage : MonoBehaviour
    {
        // The name of the stage.
        public string stageName = "";

        [Header("Default Assets")]
        // The test camera for the stage. This gets deleted when the stage is loaded in so that there aren't two main cameras.
        public Camera testCamera;

        // Start is called before the first frame update
        void Start()
        {
            // Destroys the stage camera.
            if(testCamera != null)
                Destroy(testCamera.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}