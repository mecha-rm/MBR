using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // A script for the camera that follows the player.
    public class FollowerCamera : MonoBehaviour
    {
        // The camera that this script is attached to.
        public new Camera camera;

        // The target the camera is focused on.
        public GameObject target;

        // The offset of the target's position.
        public Vector3 posOffset;

        // Automatically calculates the positon offset of the target.
        public bool autoSetPosOffset = true;

        // Start is called before the first frame update
        void Start()
        {
            // Grabs the camera component on the object.
            if (camera == null)
                camera = GetComponent<Camera>();

            // If the position offset should be auto set.
            if(autoSetPosOffset && target != null)
            {
                posOffset = transform.position - target.transform.position;
            }
        }
        
        // Offset's the camera's position from the target.
        public void OffsetPositionFromTarget()
        {
            transform.position = target.transform.position + posOffset;
        }

        // Update is called once per frame
        void Update()
        {
            OffsetPositionFromTarget();
        }
    }
}
