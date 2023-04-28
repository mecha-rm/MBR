using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mbs
{
    // The manager for gameplay operations.
    public class GameplayManager : MonoBehaviour
    {
        // The instancce of the GameplayManager.
        public static GameplayManager instance;

        // Used to initialize the singleton.
        private bool initialized = false;

        // The ground tag.
        public static string GROUND_TAG = "Ground";

        // Set to 'true' to load stage in Initialize() function.
        public bool loadStage = true;

        // The player for the game.
        public Player player;

        // The finish area of the game.
        public Goal goal;

        // A parent object that contains all the debug assets. If a level is loaded successfully, the object is deleted.
        public GameObject defaultAssets;

        [Header("Timer")]

        // The timer for the game. This tracks how long the player has been going through the level for.
        public float timer = 0.0F;

        // Checks if the timer is paused or not.
        public bool pausedTimer = false;

        // Constructor
        private GameplayManager()
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
            if (!initialized)
            {
                initialized = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Finds the player.
            if (player == null)
                player = FindObjectOfType<Player>();

            // Finds the finish.
            if (goal == null)
                goal = FindObjectOfType<Goal>();


            // Initializes the game.
            Initialize();
        }

        // Returns the instance of the gameplay manager.
        public static GameplayManager Instance
        {
            get
            {
                // Instance does not exist.
                if (instance == null)
                {
                    GameObject manager = new GameObject("Gameplay Manager (singleton)");
                    instance = manager.AddComponent<GameplayManager>();
                }

                // Return instance.
                return instance;
            }
        }

        // Initializes the gameplay manager.
        private void Initialize()
        {
            // Loads the stage.
            if(loadStage)
            {
                // Finds the stage start object.
                StageStart start = FindObjectOfType<StageStart>();

                // Stage does not exist. Throw error.
                if (start == null)
                {
                    Debug.LogError("The stage could not be found.");
                    // ...
                }

                // No stage scene has been set.
                if (start.stageScene == string.Empty)
                {
                    Debug.LogError("No scene set.");
                    // ...
                }
                else
                {
                    // Adds the provided scene to the current scene.
                    // TODO: load the scene asynchronously.
                    SceneManager.LoadScene(start.stageScene, LoadSceneMode.Additive);

                    // Destoys all the default assets.
                    if(defaultAssets != null)
                        Destroy(defaultAssets);
                }
            }


            // Spawn Transforms
            // Finds the player and goal spawn.
            PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>(true);
            GoalSpawn goalSpawn = FindObjectOfType<GoalSpawn>(true);

            // Player spawn found.
            if(playerSpawn != null)
            {
                // Gets the player transform for the stage.
                player.transform.position = playerSpawn.transform.position;
                player.transform.rotation = playerSpawn.transform.rotation;
            }


            // Goal spawn found.
            if (goalSpawn != null)
            {
                // Gets the goal transform for the stage.
                goal.transform.position = goalSpawn.transform.position;
                goal.transform.rotation = goalSpawn.transform.rotation;
            }

            // ... Do more.
        }


        // Called when the player has finished the course.
        public void OnFinish()
        {
            // TODO: show results screen
            SceneManager.LoadScene("TitleScene");
        }

        // Update is called once per frame
        void Update()
        {
            // Increases the timer.
            if(!pausedTimer)
            {
                timer += Time.deltaTime;
            }
        }
    }
}