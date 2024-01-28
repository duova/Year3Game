using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Module
{
    public class FiringModule : Module
    {
        [SerializeField]
        private ModuleSlotType slotType;

        //Locations/rotations to launch the projectiles from. Fired in order.
        [SerializeField]
        private GameObject[] shotOriginTransformMarkers;

        //Time between firing projectiles within a burst (where one projectile is fired from each marker).
        [SerializeField]
        private float burstInterval;

        [SerializeField]
        private float timeBetweenBurst;

        [SerializeField]
        private float engagementRange;

        [SerializeField]
        private GameObject shotPrefab;

        [SerializeField]
        private GameObject rotatedObject;

        [SerializeField]
        private bool isAoE;
        
        private bool _simulationTicker;

        private Entity _target;
        
        private float _burstCooldown;

        private float _intervalCooldown;

        private int _shotsRemainingInBurst;
        
        public override IEnumerable<ModuleSlotType> GetModuleTypes()
        {
            return new[] { slotType };
;        }

        public override bool IsAreaOfEffectAttack()
        {
            return isAoE;
        }

        private void Update()
        {
            _burstCooldown -= Time.deltaTime;
            _intervalCooldown -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (MatchManager.Instance.MatchState != MatchState.Simulation) return;
            
            //Only run on every other physics update to reduce lag from querying on tick.
            _simulationTicker = !_simulationTicker;
            
            if (_simulationTicker) return;

            _target = Slot.Entity.OrderedEnemyList[0];

            if (_target != null)
            {
                //Rotate to point at target with parabolic arc.
                rotatedObject.transform.rotation = Quaternion.LookRotation(ArcUtility.ArcToTarget(transform.position, _target.transform.position), Vector3.up);
            }

            if (Slot.Entity.IsInRange(_target, engagementRange) && _burstCooldown < 0f)
            {
                //Gain charges.
                _shotsRemainingInBurst = shotOriginTransformMarkers.Length;
                //Fire burst.
                if (_shotsRemainingInBurst > 0 && _intervalCooldown < 0f)
                {
                    //Fire charge.
                    _shotsRemainingInBurst--;
                    var proj = Instantiate(shotPrefab, shotOriginTransformMarkers[^_shotsRemainingInBurst].transform);
                    proj.transform.parent = null;
                    _intervalCooldown = burstInterval;
                }
                
                //Set cooldown with added time for the burst.
                _burstCooldown = timeBetweenBurst + (shotOriginTransformMarkers.Length - 1) * burstInterval;
            }
        }
    }
}