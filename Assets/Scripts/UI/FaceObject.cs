using System;
using UnityEngine;

namespace UI
{
    public class FaceObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToFace;

        [SerializeField]
        private bool invertDirection;
        
        [SerializeField]
        private bool useCameraAsObject;
        
        [SerializeField]
        private bool onlyCopyRotation;

        private void Start()
        {
            if (useCameraAsObject)
            {
                objectToFace = Camera.main.gameObject;
            }
        }

        private void Update()
        {
            if (!onlyCopyRotation)
            {
                transform.rotation = Quaternion.LookRotation(
                    invertDirection
                        ? transform.position - objectToFace.transform.position
                        : objectToFace.transform.position - transform.position, Vector3.up);
            }
            else
            {
                transform.rotation = objectToFace.transform.rotation;
            }
        }
    }
}