using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Moves platforms with objects as waypoints.
    public class MovingPlatformWithObjects : MovingPlatform
    {
        // The list of waypoints.
        public List<GameObject> waypoints;

        // Gets the requested waypoint.
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