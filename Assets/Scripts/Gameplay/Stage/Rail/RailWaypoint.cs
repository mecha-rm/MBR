using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // A waypoint on the rail.
    public class RailWaypoint : MonoBehaviour
    {
        // The start control point of the waypoint.
        // This control point is used when this waypoint is the start of a segment.
        public Vector3 controlPointStart;

        // The end control point of the waypoint.
        // This contorl point is used when this waypoint is the end of a segment.
        public Vector3 controlPointEnd;

        // Gets the world position of the start control point.
        public Vector3 GetControlPointStartWorldPosition()
        {
            Vector3 wPos = transform.position + controlPointStart;
            return wPos;
        }

        // Gets the world position of the end control point.
        public Vector3 GetControlPointEndWorldPosition()
        {
            Vector3 wPos = transform.position + controlPointEnd;
            return wPos;
        }
    }
}
