using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbr
{
    // A script that's used to attach a rail rider to a rail.

    public class RailRider : MonoBehaviour
    {
        // The rail the rider is attached to.
        public Rail rail;

        // The start point and end points.
        public RailWaypoint startPoint;
        public RailWaypoint endPoint;

        // // The t-value for going along the rail. - OLD
        // public float railT = 0.0F;

        // The speed of the rail rider.
        public float speed = 1.0F;

        // TODO: edit rigidbody for travelling along rails (not required to have component).
        public new Rigidbody rigidbody;

        // CALLBACKS
        // A callback for the rail rider.
        public delegate void RailRiderCallback(Rail rail, RailRider rider);

        // Callback for the rail rider being attached.
        private RailRiderCallback attachedCallback;

        // Callback for the rail rider being detached.
        private RailRiderCallback detachedCallback;

        // OnPositionUpdated callback.
        public delegate void RailPositionCallback(Rail rail, RailRider rider, Vector3 oldPos, Vector3 newPos);

        // The position callback.
        private RailPositionCallback positionCallback;

        // Start is called before the first frame update
        void Start()
        {
            // Rigidbody not set.
            if(rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
        }

        // Adds a callback for when the rail rider is attached to the rail.
        public void OnAttachToRailAddCallback(RailRiderCallback callback)
        {
            attachedCallback += callback;
        }

        // Removes a callback for when the rail rider becomes attached to a rail.
        public void OnAttachToRailRemoveCallback(RailRiderCallback callback)
        {
            attachedCallback -= callback;
        }

        // Called when the rail has been attached.
        public void OnAttachToRail(Rail rail)
        {
            this.rail = rail;

            if (attachedCallback != null)
                attachedCallback(rail, this);
        }

        // Attach to the rail.
        public bool AttachToRail()
        {
            bool result = false;

            // Tries to attach to the rail.
            if (rail != null)
                result = rail.TryAttachToRail(gameObject);

            return result;
        }

        // Adds a callback for when the rail rider is detached from the rail.
        public void OnDetachFromRailAddCallback(RailRiderCallback callback)
        {
            detachedCallback += callback;
        }

        // Removes a callback for when the rail rider becomes detached from a rail.
        public void OnDetachFromRailRemoveCallback(RailRiderCallback callback)
        {
            detachedCallback -= callback;
        }

        // Called when the rail has been detached.
        public void OnDetachFromRail(Rail rail)
        {
            this.rail = rail;

            if (detachedCallback != null)
                detachedCallback(rail, this);
        }

        // Detach from the rail.
        public bool DetachFromRail()
        {
            bool result = false;

            // Tries to detach from the rail.
            if (rail != null)
            {
                // NOTE: you may not want to multiply by Time.deltaTime since this is a one-time force...
                // But otherwise it appears to jump forward too fast.

                // Old - generates an even amount of force regardless of where the rider is on the rail.
                // The movement direction.
                Vector3 direc = (endPoint.transform.position - startPoint.transform.position).normalized;
                
                // The force generated.
                Vector3 posStep = direc.normalized * rail.speed * speed * Time.deltaTime;
                
                // The simulated old position.
                Vector3 simOldPos = transform.position - posStep;
                
                // Calculates the position.
                result = rail.TryDetachFromRail(this, simOldPos);

                // // New - uses MoveTowards - works about the same, but on corners the jump forward force is reduced.
                // float step = rail.speed * speed * Time.deltaTime;
                // result = rail.TryDetachFromRail(this, Vector3.MoveTowards(transform.position, startPoint.transform.position, step));
            }

            return result;
        }

        // Checks if a rider is attached to their set rail.
        public bool IsAttachedToRail()
        {
            // Checks if the rail is set.
            if(rail == null)
            {
                return false;
            }
            else
            {
                // Check if connected.
                bool result = rail.IsRiderAttached(this);
                return result;
            }
        }

        // Adds a callback for when the rail rider positon is updated.
        public void OnPositionUpdatedAddCallback(RailPositionCallback callback)
        {
            positionCallback += callback;
        }

        // Removes a callback for when the rail rider position is updated.
        public void OnPositionUpdatedRemoveCallback(RailPositionCallback callback)
        {
            positionCallback -= callback;
        }

        // Called when the position has been updated. This is called at the end of the position update of the rail rider.
        public virtual void OnPositionUpdated(Vector3 oldPos, Vector3 newPos)
        {
            // Old position, new position.
            if (positionCallback != null)
                positionCallback(rail, this, oldPos, newPos);
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}