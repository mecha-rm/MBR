using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
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

        // Checks what axes should be followed.
        public bool followX = true;
        public bool followY = true;
        public bool followZ = true;

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
            // The new position.
            Vector3 newPos;

            // Checks if the position should be changed or not.
            newPos.x = followX ? target.transform.position.x + posOffset.x : transform.position.x;
            newPos.y = followY ? target.transform.position.y + posOffset.y : transform.position.y;
            newPos.z = followZ ? target.transform.position.z + posOffset.z : transform.position.z;

            // transform.position = target.transform.position + posOffset; // Original
            transform.position = newPos;
        }

        // LateUpdate is called once per frame, if the behaviour is enabled.
        // Since this happens after all updates, it's recommended that follower cameras use this.
        // Rigidbodies don't update until the update calls are finished, so it's best to use LateUpdate for that reason as well.
        void LateUpdate()
        {
            OffsetPositionFromTarget();
        }
    }
}
