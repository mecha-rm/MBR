using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // A script that's used to attach a rail rider to a rail.

    public class RailRider : MonoBehaviour
    {
        // The rail the rider is attached to.
        public Rail rail;

        // The start point and end points.
        public GameObject startPoint;
        public GameObject endPoint;

        // The t-value for going along the rail.
        public float railT = 0.0F;

        // The speed of the rail rider.
        public float speed = 1.0F;

        // TODO: edit rigidbody for travelling along rails (not required to have component).
        public new Rigidbody rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            // Rigidbody not set.
            if(rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
        }

        // Called when the position has been updated. This is called at the end of the position update of the rail rider.
        public void OnPositionUpdated()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}