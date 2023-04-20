using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // This object is created to provide information on how to start a stage. 
    public class StageStart : MonoBehaviour
    {
        // The stage scene to be loaded.
        // The stage loaded will be additive to the game scene.
        public string stageScene = "";

        // Start is called before the first frame update
        void Start()
        {
            // Don't destroy this object so that the GameScene can find it.
            DontDestroyOnLoad(this);
        }
    }
}