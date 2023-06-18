using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // The stage virtual camera.
    public class StageVirtualCamera : MonoBehaviour
    {
        // The cinemachine virtual camera.
        public CinemachineVirtualCamera vcam;

        [Header("Settings")]

        // Applies the camera settings upon it being activated.
        [Tooltip("Applies the settings when the camera is activated.")]
        public bool applySettings = true;

        // Applies the movement mode of this camera.
        public bool applyMoveMode = true;

        // The movement mode of the virtual camera.
        public Player.MovementMode moveMode;

        // Applies the target's rotation.
        public bool applyRotation = true;

        // The rotation of the player's camera.
        public Quaternion targetRot = Quaternion.identity;

        [Header("Target")]
        // The target for the virtual camera.
        public Transform target;

        // The target's parent for when the camera is deactivated.
        [Tooltip("The original parent of the target.")]
        public Transform targetParent = null;

        // If 'true', the target is made the child of the transform copy. If false, it's made to the player.
        [Tooltip("If true, the target is childed to the player's transform copy. If false, the player is made the parent.")]
        public bool childToTransformCopy = true;

        // The target position offset when parented.
        public Vector3 targetPosOffset = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            // Tries to grab the vcam if it's not set.
            if(vcam == null)
                vcam = GetComponent<CinemachineVirtualCamera>();


            // Sets the target parent.
            if (target != null)
                targetParent = target.parent;
        }

        // This function is called when the object becomes enabled and active.
        private void OnEnable()
        {
            OnCameraActivate(childToTransformCopy);
        }

        // This function is called when this behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            OnCameraDeactivate();
        }

        // Called when the camera is activated.
        // If 'setToTransformCopy' is true, the target is made the child of the transform copy.
        // If 'setToTransformCopy' if false, the target is made the child of the player itself.
        public void OnCameraActivate(bool setToTransformCopy)
        {
            // No target set.
            if (target == null)
                return;

            // Grabs hte gameplay manager.
            GameplayManager manager = GameplayManager.Instance;

            // Set the target transform parent.
            if(setToTransformCopy) // Set to transform copy.
            {
                // Set the transform.
                if (manager.player.cameraTarget != null)
                    target.transform.parent = manager.player.cameraTarget.transform;
                else
                    target.transform.parent = manager.player.transform;
            }
            else // Set to player directly.
            {
                target.transform.parent = manager.player.cameraTarget.transform;
            }

            // Set the local position to zero, then apply the target offset.
            target.localPosition = Vector3.zero;
            target.localPosition = targetPosOffset;


            // Apply the camera settings.
            if(applySettings)
                ApplyCameraSettings();
        }

        // Called when the camera is deactivated.
        public void OnCameraDeactivate()
        {
            // No target set.
            if (target == null)
                return;

            // Set the target back to its base parent.
            target.transform.parent = targetParent;
        }

        // Applies the camera settings.
        public void ApplyCameraSettings()
        {
            // Apply Camera Settings
            Player player = GameplayManager.Instance.player;

            // Set the movement mode.
            if (applyMoveMode) // Movement mode.
            {
                player.SetMovementMode(moveMode);
            }

            // Set the rotation.
            if (applyRotation) // Rotation.
            {
                player.cameraTarget.transform.rotation = targetRot;
            }
        }
    }
}