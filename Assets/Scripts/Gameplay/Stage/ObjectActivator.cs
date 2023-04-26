using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Activates objects upon the object being triggred.
    public class ObjectActivator : MonoBehaviour
    {
        // Determines if the object's are activated or de-activated.
        [Header("The value put in for SetActive() on the list objects.")]
        public bool active = true;

        // Destroys this object upon setting the active parameter of the objects.
        public bool destroyOnSetActive = true;

        // The list of connected objects.
        public List<GameObject> objects = new List<GameObject>();

        // The tags that are used to trigger the collision. If no tags are included, then any object can trigger it.
        public List<string> tags = new List<string>();

        // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        private void OnCollisionEnter(Collision collision)
        {
            if(tags.Count == 0 || tags.Contains(collision.gameObject.tag))
                SetObjectsActive(active);
        }

        // OnTriggerEnter is called when the Collider other enters the trigger.
        private void OnTriggerEnter(Collider other)
        {
            if (tags.Count == 0 || tags.Contains(other.gameObject.tag))
                SetObjectsActive(active);
        }

        // Sets the objects active. If 'overrideActive' is true, the value is saved to 'active'
        public void SetObjectsActive(bool value, bool overrideActive = true)
        {
            // Overwrite the active variable.
            if (overrideActive)
                active = value;

            // Calls SetActive on the objects using the provided value.
            foreach (GameObject obj in objects)
                obj.SetActive(value);


            // Destroys this game object.
            if (destroyOnSetActive)
                Destroy(gameObject);
        }
    }
}