using Cinemachine;
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
        private static GameplayManager instance;

        // Used to initialize the singleton.
        private bool instanced = false;

        // Gets set to 'true' when the game has been initialized.
        private bool initialized = false;

        // Delays game initialization by providing the amount of frame delays.
        // NOTE: for SOME reason, objects only become detactable from the additive scene...
        // from Update call 2 onwards. So I need to do this workaround to initialize the game properly.
        private int initFrameDelay = 1;

        // The ground tag.
        public static string GROUND_TAG = "Ground";

        // A parent object that contains all the debug assets. If a level is loaded successfully, the object is deleted.
        public GameObject defaultAssets;

        [Header("Stage")]

        // The game stage.
        public Stage stage = null;

        // The stage being loaded.
        public string stageScene = "";

        // Set to 'true' to load stage in Initialize() function.
        public bool loadStage = true;

        // The y-value of the death plane. Anything below this plane is considered 'dead', and thus will be destroyed.
        public float deathPlaneY = -30.0F;

        [Header("Player")]

        // The player for the game.
        public Player player;

        // The player's spawn point.
        public PlayerSpawn playerSpawn;

        // The active virtual camera.
        public CinemachineVirtualCamera activeVcam;

        [Header("Progress")]

        // The finish area of the game.
        public Goal goal;

        // The goal spawn point.
        public GoalSpawn goalSpawn;

        // The checkpoint that the player respawns at.
        public Checkpoint setCheckpoint = null;

        // TODO: implement checkpoint and make sure to give it the respawn camera.

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
            if (!instanced)
            {
                instanced = true;
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

            // Finds the active virtual camera.
            if (activeVcam == null)
                activeVcam = FindObjectOfType<CinemachineVirtualCamera>(false);


            // It appears that you can't get things properly if you loaded a scene on the same update where you're getting objects...
            // So you moved this here.
            // Loads the stage.
            if (loadStage)
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
                    stageScene = start.stageScene;

                    // TODO: load the scene asynchronously.
                    SceneManager.LoadScene(start.stageScene, LoadSceneMode.Additive);

                    // Destroys the start scene.
                    Destroy(start);

                    // Destoys all the default assets.
                    if (defaultAssets != null)
                    {
                        Destroy(defaultAssets);
                    }
                        
                }
            }
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
            // Stage Info
            if(stage == null)
                stage = FindObjectOfType<Stage>(true);


            // Spawn Transforms
            // Finds the player and goal spawn.
            playerSpawn = FindObjectOfType<PlayerSpawn>(true);
            goalSpawn = FindObjectOfType<GoalSpawn>(true);

            // Player spawn found.
            if (playerSpawn != null)
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

            // The game has been initialized.
            initialized = true;
        }

        // Switches over to the provided camera.
        public void SetVirtualCamera(CinemachineVirtualCamera newVcam)
        {
            // Turns off the active camera.
            if (activeVcam != null)
                activeVcam.gameObject.SetActive(false);

            // Makes sure the new virtual camera is active.
            newVcam.gameObject.SetActive(true);

            // Save the new virtual camera.
            activeVcam = newVcam;
        }

        // Called when an object has reached the death plane.
        public void OnDeathPlaneReached(GameObject entity)
        {
            // The entity is a player.
            if(entity.tag == "Player")
            {
                Player p;

                // Grab the player component.
                if(entity.TryGetComponent(out p))
                {
                    // Cancel the velocity.
                    p.rigidbody.velocity = Vector3.zero;

                    // TODO: handle cameras.

                    // Put the player at the set checkpoint.
                    if(setCheckpoint != null)
                    {
                        // Return to the respawn position.
                        setCheckpoint.RespawnAtCheckpoint(p.gameObject, true);
                    }
                    else
                    {
                        p.transform.position = Vector3.zero;
                    }
                }
            }
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
            PlayerSpawn ps = FindObjectOfType<PlayerSpawn>(true);
            GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

            // Initializes the game.
            if (!initialized)
            {
                // Reduce the init frame delay.
                if (initFrameDelay > 0)
                {
                    initFrameDelay--;

                    // Negative, so set to 0.
                    if (initFrameDelay < 0)
                        initFrameDelay = 0;
                }
                else
                {
                    // The frame delay is 0, so now initialize the game.
                    initFrameDelay = 0;
                    Initialize();
                }
            }
                

            // Increases the timer.
            if(!pausedTimer)
            {
                timer += Time.deltaTime;
            }
        }
    }
}