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

        // Rotates the 2D vector (around its z-axis).
        public static Vector2 Rotate(Vector2 v, float angle, bool inDegrees)
        {
            // Converts the angle to radians if provided in degrees.
            angle = (inDegrees) ? Mathf.Deg2Rad * angle : angle;

            // Calculates the rotation using matrix math...
            // Except it manually puts in what the resulting calculation would be).
            Vector2 result;
            result.x = (v.x * Mathf.Cos(angle)) - (v.y * Mathf.Sin(angle));
            result.y = (v.x * Mathf.Sin(angle)) + (v.y * Mathf.Cos(angle));

            return result;
        }

        // The rotation matrix.
        private static Vector3 Rotate(Vector3 v, float angle, char axis, bool inDegrees)
        {
            // Converts the angle to radians if provided in degrees.
            angle = (inDegrees) ? Mathf.Deg2Rad * angle : angle;

            // The rotation matrix.
            Matrix4x4 rMatrix = new Matrix4x4();

            // Checks what axis to rotate the vector3 on.
            switch (axis)
            {
                case 'x': // X-Axis
                case 'X':
                    // Rotation X Matrix
                    /*
                     * [1, 0, 0, 0]
                     * [0, cos a, -sin a, 0]
                     * [0, sin a, cos a, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
                    rMatrix.SetRow(1, new Vector4(0, Mathf.Cos(angle), -Mathf.Sin(angle), 0));
                    rMatrix.SetRow(2, new Vector4(0, Mathf.Sin(angle), Mathf.Cos(angle), 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                case 'y': // Y-Axis
                case 'Y':
                    // Rotation Y Matrix
                    /*
                     * [cos a, 0, sin a, 0]
                     * [0, 1, 0, 0]
                     * [-sin a, 0, cos a, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(Mathf.Cos(angle), 0, Mathf.Sin(angle), 0));
                    rMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
                    rMatrix.SetRow(2, new Vector4(-Mathf.Sin(angle), 0, Mathf.Cos(angle), 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                case 'z': // Z-Axis
                case 'Z':
                    // Rotation Z Matrix
                    /*
                     * [cos a, -sin a, 0, 0]
                     * [sin a, cos a, 0, 0]
                     * [0, 0, 1, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(Mathf.Cos(angle), -Mathf.Sin(angle), 0, 0));
                    rMatrix.SetRow(1, new Vector4(Mathf.Sin(angle), Mathf.Cos(angle), 0, 0));
                    rMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                default: // Unknown
                    return v;
            }



            // The vector matrix.
            Matrix4x4 vMatrix = new Matrix4x4();
            vMatrix[0, 0] = v.x;
            vMatrix[1, 0] = v.y;
            vMatrix[2, 0] = v.z;
            vMatrix[3, 0] = 1;


            // The resulting matrix.
            Matrix4x4 resultMatrix = rMatrix * vMatrix;

            // Gets the vector3 from the result matrix.
            Vector3 resultVector = new Vector3(
                resultMatrix[0, 0],
                resultMatrix[1, 0],
                resultMatrix[2, 0]
                );

            // Returns the result.
            return resultVector;
        }

        // Rotate around the x-axis.
        public static Vector3 RotateX(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'X', inDegrees);
        }

        // Rotate around the y-axis.
        public static Vector3 RotateY(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'Y', inDegrees);
        }

        // Rotate around the z-axis.
        public static Vector3 RotateZ(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'Z', inDegrees);
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