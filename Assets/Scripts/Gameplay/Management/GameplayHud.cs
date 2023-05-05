using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace mbs
{
    // The gameplay hud.
    public class GameplayHud : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        // The current game time.
        public TMP_Text time;

        // Start is called before the first frame update
        void Start()
        {
            // Finds the instance of the gameplay manager.
            if(gameManager == null)
                gameManager = GameplayManager.Instance;
        }

        // Formats the time string.
        public string FormatTime(float timeSeconds, float maxSeconds = 5999.0F)
        {
            // Calculates the total time, limiting it to 99 miuntes and 59 seconds.
            // Max Time = 60 * 99 + 59 = 5940 + 59 = 5999 [99:59]
            float totalTime = Mathf.Clamp(timeSeconds, 0, maxSeconds); // total time in seconds.

            // Minutes and seconds - hours isn't used since it's unnecessary.
            float minutes = Mathf.Floor(totalTime / 60.0F); // minutes (floor round to remove seconds).
            float seconds = Mathf.Ceil(totalTime - (minutes * 60.0F)); // seconds (round up to remove nanoseconds).

            // Sets the text.
            string timeString = minutes.ToString("00") + ":" + seconds.ToString("00");

            // Returns ther esults.
            return timeString;
        }

        // Update is called once per frame
        void Update()
        {
            // Sets the time text.
            time.text = FormatTime(gameManager.timer);
        }
    }
}