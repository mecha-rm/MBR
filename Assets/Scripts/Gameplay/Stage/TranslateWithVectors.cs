using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace mbs
{
    // Translates with vectors.
    public class TranslateWithVectors : Translation
    {
        // The list of waypoints.
        public List<Vector3> waypoints = new List<Vector3>();

        // Returns the requested waypoint.
        public override Vector3 GetWaypoint(int index)
        {
            return waypoints[index];
        }

        // Gets the waypoint count.
        public override int GetWaypointCount()
        {
            return waypoints.Count;
        }
    }
}