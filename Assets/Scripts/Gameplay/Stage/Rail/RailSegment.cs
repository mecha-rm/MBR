using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // Acts as a collision point for a rail.
    public class RailSegment : MonoBehaviour
    {
        // The rail this segment belongs to.
        public Rail rail;

        // Start is called before the first frame update
        void Start()
        {
            // Tries to get the rail component from the parent.
            if (rail == null)
                rail = gameObject.GetComponentInParent<Rail>();
        }

        // OnCollisionEnter is called when a collider/rigidbody has begun touching another collider/rigidbody.
        private void OnCollisionEnter(Collision collision)
        {
            if(rail != null)
                rail.TryAttachToRail(collision.gameObject);
        }

        // // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        // private void OnCollisionExit(Collision collision)
        // {
        //     if (rail != null)
        //         rail.TryDetachFromRail(collision.gameObject);
        // }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider collision)
        {
            if (rail != null)
                rail.TryAttachToRail(collision.gameObject);
        }

        // // OnTriggerExit is called when this Collider other has stopped touching the trigger.
        // private void OnTriggerExit(Collider collision)
        // {
        //     if (rail != null)
        //         rail.TryDetachFromRail(collision.gameObject);
        // }
    }
}