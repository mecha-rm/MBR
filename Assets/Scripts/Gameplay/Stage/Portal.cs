using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // A portal that takes the valid tags to another location.
    public class Portal : MonoBehaviour
    {
        // Determines if an end portal is used or not.
        public bool useEndPortal = true;

        [Header("One-Way")]
        // The destination of the one-way portal.
        public Vector3 destination;

        [Header("End Portal")]
        // The destination portal.
        public Portal destPortal;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}