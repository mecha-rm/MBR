using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // A checkpoint for respawning the player.
    public class Checkpoint : MonoBehaviour
    {
        // The offset from the checkpoint's position, which is used as the respawn point.
        public Vector3 respawnOffset;

        // The camera set for this checkpoint.
        public CinemachineVirtualCamera vcam;

        // Gets set to 'true' when a checkpoint is activated. A checkpoint cannot be set as the main checkpoint twice.
        public bool activated = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            // Save this as the most recent checkpoint if it hasn't been activated yet.
            if(!activated)
            {
                // Proper tag.
                if (collision.gameObject.tag == Player.PLAYER_TAG)
                    ActivateCheckpoint();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Save this as the most recent checkpoint if it hasn't been activated yet.
            if (!activated)
            {
                // Proper tag.
                if (other.gameObject.tag == Player.PLAYER_TAG)
                    ActivateCheckpoint();
            }
        }

        // Sets this as the most recent checkpoint.
        public void ActivateCheckpoint()
        {
            activated = true;
            GameplayManager.Instance.setCheckpoint = this;
        }

        // Gets the respawn position.
        public Vector3 GetRespawnPosition()
        {
            return transform.position + respawnOffset;
        }

        // Respawns the entity at the checkpoint.
        // If 'setCamera' is true, then the current camera is set to the checkpoint's vcam.
        public void RespawnAtCheckpoint(GameObject entity, bool setCamera)
        {
            // Move to respawn position.
            entity.transform.position = GetRespawnPosition();

            // Checks if the camera is set.
            if(vcam != null)
            {
                // Grabs the gameplay manager.
                GameplayManager gm = GameplayManager.Instance;

                // Checks if the camera is set.
                if (gm.activeVcam != null)
                {
                    gm.activeVcam.gameObject.SetActive(false);
                }
                else // Find and turn off camera.
                {
                    // Finds the active vcam.
                    CinemachineVirtualCamera currVcam = FindObjectOfType<CinemachineVirtualCamera>(false);

                    // Turns off the current camera.
                    currVcam.gameObject.SetActive(false);
                }

                // Turns on the checkpoint's camera.
                vcam.gameObject.SetActive(true);

                // Saves it to the gameplay manager.
                gm.activeVcam = vcam;
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}