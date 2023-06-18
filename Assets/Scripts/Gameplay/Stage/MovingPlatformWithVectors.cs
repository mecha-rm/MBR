using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // Moves platform with vectors as waypoints.
    public class MovingPlatformWithVectors : MovingPlatform
    {
        // The list of waypoints.
        public List<Vector3> waypoints;

        // Gets the requested waypoint.
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