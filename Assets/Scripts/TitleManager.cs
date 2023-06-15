using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mbs
{
    // The manager for the title screen of the game.
    public class TitleManager : MonoBehaviour
    {
        // The instance of the game.
        private static TitleManager instance;

        // Used for initializing the singleton.
        private bool instanced = false;

        // Constructor
        private TitleManager()
        {

        }

        // Awake is called when the script instance is loaded.
        private void Awake()
        {
            // If the instance is not set.
            if (instance == null)
            {
                instance = this;
            }


            // Implement any Awake code here for initializing the singleton.
            if(!instanced)
            {
                instanced = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Returns the instance of the title manager.
        public static TitleManager Instance
        {
            get
            {
                // Instance does not exist.
                if (instance == null)
                {
                    GameObject manager = new GameObject("Title Manager (singleton)");
                    instance = manager.AddComponent<TitleManager>();
                }

                // Return instance.
                return instance;
            }
        }

        // Loads the game scene - TODO: load asynchronously, and have user select stage. 
        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}