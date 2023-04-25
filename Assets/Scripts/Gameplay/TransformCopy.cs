using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Copies the transform of an object.
    public class TransformCopy : MonoBehaviour
    {
        // The target to copy.
        public GameObject target;

        // Copy the target's information.
        public bool autoCopy = true;

        [Header("Settings")]
        // Copies the position.
        public bool copyPositionX = true;
        public bool copyPositionY = true;
        public bool copyPositionZ = true;

        // Copies the rotation.
        public bool copyRotationX = true;
        public bool copyRotationY = true;
        public bool copyRotationZ = true;

        // Copies the scale.
        public bool copyScaleX = true;
        public bool copyScaleY = true;
        public bool copyScaleZ = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Copies the target's information.
        public void UpdateTransform()
        {
            // Target transform information.
            Vector3 targetPos = target.transform.position;
            Vector3 targetRot = target.transform.eulerAngles;
            Vector3 targetScl = target.transform.localScale;

            // New transform information.
            Vector3 newPos = targetPos;
            Vector3 newRot = targetRot;
            Vector3 newScl = targetScl;

            // The transform information sets.
            bool[] posSets = { copyPositionX, copyPositionY, copyPositionZ };
            bool[] rotSets = { copyRotationX, copyRotationY, copyRotationZ };
            bool[] sclSets = { copyScaleX, copyScaleY, copyScaleZ };

            // Position Setting
            for (int i = 0; i < posSets.Length; i++)
                newPos[i] = posSets[i] ? newPos[i] : transform.position[i];

            // Rotation Setting
            for (int i = 0; i < rotSets.Length; i++)
                newRot[i] = rotSets[i] ? newRot[i] : transform.eulerAngles[i];

            // Scale Setting
            for (int i = 0; i < sclSets.Length; i++)
                newScl[i] = sclSets[i] ? newScl[i] : transform.localScale[i];


            // Set the transform information.
            transform.position = newPos;
            transform.eulerAngles = newRot;
            transform.localScale = newScl;
        }

        // LateUpdate is called every frame, if the behaviour is enabled.
        void LateUpdate()
        {
            // If the transform should be copied automatically.
            if(autoCopy)
            {
                UpdateTransform();
            }
        }
    }
}