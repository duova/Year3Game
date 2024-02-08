using System;
using UnityEngine;
using UnityEngine.Serialization;
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
        private float velocity;

        private float _travelTime;

        private void Start()
        {
            if (velocity == 0) velocity = 1;

            transform.rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            
            var distance = (Target.transform.position - transform.position).magnitude;

            var distanceAttribute = shotVfx.CreateVFXEventAttribute();
            distanceAttribute.SetFloat("distance", distance);
            
            var velocityAttribute = shotVfx.CreateVFXEventAttribute();
            velocityAttribute.SetFloat("velocity", velocity);

            _travelTime = distance / velocity;
        }

        private void Update()
        {
            _travelTime -= Time.deltaTime;
            if (_travelTime < 0)
            {
                if (Target)
                {
                    Target.AddHealth(-damage);
                }

                if (Origin)
                {
                    Origin.AddHealth(damage * lifesteal);
                }

                Destroy(gameObject);
            }
        }
    }
}