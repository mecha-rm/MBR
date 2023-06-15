using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// The virtual camera switcher.
namespace mbs
{
    public class VirtualCameraSwitcher : MonoBehaviour
    {
        // The two cameras to switch between. VCAM1 is the primary one, and is active first.
        public CinemachineVirtualCamera vcam1;
        public CinemachineVirtualCamera vcam2;

        // Switches cameras when the collision is entered.
        [Tooltip("If true, the cameras are switched when the collider is entered.")]
        public bool switchOnEnter = true;

        // Switches cameras when the collision is exited.
        [Tooltip("If true, the cameras are switched when the collider is exited.")]
        public bool switchOnExit = false;

        // Sets the active parameter of the whole object. If false, it only changes the component.
        [Tooltip("If true, vcam.gameObject.activeSelf is changed. If false, component.enabled is changed.")]
        public bool setActive = true;

        // List of supported tags. If no tags are set, all tags are supported.
        [Tooltip("The list of supported tags. If no tags are provided, then any collider can trigger it.")]
        public List<string> tags = new List<string>();

        [Header("Settings")]

        // Applies settings to the game when the cameras are switched.
        public bool applySettings = true;

        [Header("Settings/Movement Mode")]

        // Applies the movement mode in the settings.
        public bool applyMoveMode = true;
        
        // The movement mode of the first virtual camera.
        public Player.MovementMode vcam1MoveMode;

        // The movement mode of the second virtual camera.
        public Player.MovementMode vcam2MoveMode;

        [Header("Settings/Rotation")]

        // Applies the rotation settings.
        public bool applyRotation = true;

        // The rotation of the player's camera target when vcam1 is activated.
        public Quaternion vcam1CamTargetRot = Quaternion.identity;

        // The rotation of the player's camera target when vcam2 is activated.
        public Quaternion vcam2CamTargetRot = Quaternion.identity;

        // On collision enter.
        private void OnCollisionEnter(Collision collision)
        {
            // If the cameras should switch when the collision is entered.
            if(switchOnEnter)
            {
                // Checks for valid tags.
                if (tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                    SwitchCameras();
            }
            
        }

        // On collision exit.
        private void OnCollisionExit(Collision collision)
        {
            // If the cameras should switch when the collision is entered.
            if (switchOnExit)
            {
                // Checks for valid tags.
                if (tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                    SwitchCameras();
            }

        }

        // On trigger enter.
        private void OnTriggerEnter(Collider other)
        {
            // If the cameras should switch when the collision is entered.
            if (switchOnEnter)
            {
                // Checks for valid tags.
                if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                    SwitchCameras();
            }
        }

        // On trigger exit.
        private void OnTriggerExit(Collider other)
        {
            // If the cameras should switch when the collision is entered.
            if (switchOnExit)
            {
                // Checks for valid tags.
                if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                    SwitchCameras();
            }
        }

        // Returns the active virtual camera. If both are active, it defaults to vcam1.
        public CinemachineVirtualCamera ActiveVCam
        {
            get
            {
                // The virtual camera.
                CinemachineVirtualCamera vcam = null;

                // Checks if vcam1 is active.
                if(vcam1.isActiveAndEnabled)
                {
                    vcam = vcam1;
                }
                // Checks if vcam2 is active.
                else if(vcam2.isActiveAndEnabled)
                {
                    vcam = vcam2;
                }
                // None are active, so set to null.
                else
                {
                    vcam = null;
                }

                return vcam;
            }
        }

        // Switch the cameras.
        public void SwitchCameras()
        {
            // The active parameter.
            bool active;
            
            // If active should be set.
            if(setActive) // Change whole object.
            {
                active = !vcam1.gameObject.activeSelf;
                vcam1.gameObject.SetActive(active);
                vcam2.gameObject.SetActive(!active);


                // Set the active camera.
                GameplayManager.Instance.activeVcam = (vcam1.gameObject.activeSelf) ? vcam1 : vcam2;
            }
            else // Change component.
            {
                active = !vcam1.enabled;
                vcam1.enabled = active;
                vcam2.enabled = !active;

                // Set the active camera.
                GameplayManager.Instance.activeVcam = (vcam1.enabled) ? vcam1 : vcam2;
            }


            // VCAM SETTINGS //
            if(applySettings)
            {
                // Gets the active virtual camera.
                CinemachineVirtualCamera vcam = ActiveVCam;

                // Gets the vcam number.
                int vcamNum = (vcam == vcam1) ? 1 : (vcam == vcam2) ? 2 : 0;

                // Gets the player.
                Player player = GameplayManager.Instance.player;

                // Change the player's movement mode.
                if(applyMoveMode)
                {
                    // Checks the vcam number to see what should be set.
                    switch(vcamNum)
                    {
                        case 1: // VCAM 1
                            player.SetMovementMode(vcam1MoveMode);
                            break;
                        case 2: // VCAM 2
                            player.SetMovementMode(vcam2MoveMode);
                            break;
                    }
                }

                // Change the player's rotation.
                if(applyRotation)
                {
                    // Checks the vcam number to see what should be set.
                    switch (vcamNum)
                    {
                        case 1: // VCAM 1
                            player.cameraTarget.transform.rotation = vcam1CamTargetRot;
                            break;
                        case 2: // VCAM 2
                            player.cameraTarget.transform.rotation = vcam2CamTargetRot;
                            break;
                    }
                }
            }           
      
        }
    }
}