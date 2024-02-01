using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Entity.Module
{
    public class HitscanShot : MonoBehaviour, IShot
    {
        public Entity Target { get; set; }
        
        public Entity Origin { get; set; }

        [SerializeField]
        private VisualEffect shotVfx;

        [SerializeField]
        private float damage;

        //Decimal fraction of damage healed.
        [SerializeField]
        private float lifesteal;

        [SerializeField]
        private float duration;

        private void Start()
        {
            var distance = (Target.transform.position - transform.position).magnitude;

            var eventAttribute = shotVfx.CreateVFXEventAttribute();
            eventAttribute.SetFloat("distance", distance);
        }

        private void Update()
        {
            duration -= Time.deltaTime;
            if (duration < 0)
            {
                Target.AddHealth(-damage);
                Origin.AddHealth(damage * lifesteal);
                Destroy(gameObject);
            }
        }
    }
}