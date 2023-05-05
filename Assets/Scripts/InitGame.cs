using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mbs
{
    // This script is used to initialize the game in the init scene.
    public class InitGame : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Sets the frame rate.
            Application.targetFrameRate = 30;
        }

        // Update is called once per frame
        void Update()
        {
            // Go to the title scene.
            SceneManager.LoadScene("TitleScene");
        }
    }
}