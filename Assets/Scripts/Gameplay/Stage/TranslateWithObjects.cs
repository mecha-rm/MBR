using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Translates using objects.
    public class TranslateWithObjects : Translation
    {
        // The list of waypoints.
        public List<GameObject> waypoints = new List<GameObject>();

        // Returns the requested waypoint.
        public override Vector3 GetWaypoint(int index)
        {
            return waypoints[index].transform.position;
        }

        // Gets the waypoint count.
        public override int GetWaypointCount()
        {
            return waypoints.Count;
        }
    }
}