using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Entity.Module
{
    public class HitscanShot : MonoBehaviour
    {
        public Entity Target { get; set; }

        [SerializeField]
        private VisualEffect shotVfx;

        [SerializeField]
        private float damage;

        private void Start()
        {
            var distance = (Target.transform.position - transform.position).magnitude;

            var eventAttribute = shotVfx.CreateVFXEventAttribute();
            eventAttribute.SetFloat("distance", distance);
        }
    }
}