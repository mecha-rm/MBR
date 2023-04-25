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

        // Sets the active parameter of the whole object. If false, it only changes the component.
        [Tooltip("If true, gameObject.activeSelf is changed. If false, component.enabled is changed.")]
        public bool setActive = true;

        // List of supported tags. If no tags are set, all tags are supported.
        [Tooltip("The list of supported tags. If no tags are provided, then any collider can trigger it.")]
        public List<string> tags = new List<string>();

        private void OnCollisionEnter(Collision collision)
        {
            // Checks for valid tags.
            if(tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                SwitchCameras();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Checks for valid tags.
            if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                SwitchCameras();
        }

        // Switch the cameras.
        public void SwitchCameras()
        {
            bool active;
            
            // If active should be set.
            if(setActive) // Change whole object.
            {
                active = !vcam1.gameObject.activeSelf;
                vcam1.gameObject.SetActive(active);
                vcam2.gameObject.SetActive(!active);
            }
            else // Change component.
            {
                active = !vcam1.enabled;
                vcam1.enabled = active;
                vcam2.enabled = !active;
            }
        }
    }
}