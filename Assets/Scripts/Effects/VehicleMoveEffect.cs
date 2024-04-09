using System;
using UnityEngine;

namespace Effects
{
    public class VehicleMoveEffect : MonoBehaviour, IMoveEffect
    {
        [SerializeField]
        private GameObject[] wheels;

        [SerializeField]
        private ParticleSystem[] particleSystems;

        [SerializeField]
        private float rate;

        private void Start()
        {
            foreach (var system in particleSystems)
            {
                system.Stop();
            }
        }

        public void BeginMove()
        {
            foreach (var system in particleSystems)
            {
                system.Play();
            }
        }

        public void MoveTick()
        {
            foreach (var wheel in wheels)
            {
                wheel.transform.localRotation *= Quaternion.Euler(0f, -rate * Time.deltaTime, 0f);
            }
        }

        public void EndMove()
        {
            foreach (var system in particleSystems)
            {
                system.Stop();
            }
        }
    }
}