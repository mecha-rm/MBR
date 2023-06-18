using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// The virtual camera switcher.
namespace mbr
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
      
        }
    }
}