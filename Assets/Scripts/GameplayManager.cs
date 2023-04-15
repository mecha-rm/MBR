using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace mbs
{
    // The manager for gameplay operations.
    public class GameplayManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Rotates a vector2.
        public static Vector2 RotateVector2(Vector2 v, float angle)
        {
            // The resulting vector.
            Vector2 result;

            // Rotations
            result.x = (v.x * Mathf.Cos(angle)) - (v.y * Mathf.Sin(angle));
            result.y = (v.x * Mathf.Sin(angle)) + (v.y * Mathf.Cos(angle));

            return result;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}