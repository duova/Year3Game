using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

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

        //0 for infinite.
        [SerializeField]
        private int burstsPerRound;

        [SerializeField]
        private float firstShotChargeTime;

        //Each burst reduces time between burst by this.
        [SerializeField]
        private float timeBetweenBurstReduction;

        //Cannot get lower than this.
        [SerializeField]
        private float minTimeBetweenBurst;

        [SerializeField]
        private float rotationRate;
        
        private bool _simulationTicker;

        private Entity _target;
        
        private float _burstCooldown;

        private float _currentTimeBetweenBurst;

        private float _intervalCooldown;

        private int _shotsRemainingInBurst;

        private int _burstsRemaining;
        
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
            if (MatchManager.Instance.MatchState != MatchState.Simulation)
            {
                //Set burst cooldown for the next round to the original.
                _currentTimeBetweenBurst = timeBetweenBurst;
                //Use cooldown as first shot charge time.
                _burstCooldown = firstShotChargeTime;

                _burstsRemaining = burstsPerRound;
                return;
            }
            
            //Only run on every other physics update to reduce lag from querying on tick.
            _simulationTicker = !_simulationTicker;
            
            if (_simulationTicker) return;

            _target = Slot.Entity.OrderedEnemyList[0];

            if (_target == null) return;
            //Rotate to point at target.
            var cachedTransform = transform;
            cachedTransform.rotation = Quaternion.RotateTowards(cachedTransform.rotation,
                    Quaternion.LookRotation(_target.transform.position - cachedTransform.position, Vector3.up),
                    rotationRate * Time.deltaTime);

            //Don't shoot if no more bursts remain.
            if (_burstsRemaining <= 0 && burstsPerRound > 0) return;
            
            if (Slot.Entity.IsInRange(_target, engagementRange) && _burstCooldown < 0f)
            {
                //Gain charges.
                _shotsRemainingInBurst = shotOriginTransformMarkers.Length;
                
                //Reduce burst cooldown.
                _currentTimeBetweenBurst = MathF.Max(_currentTimeBetweenBurst - timeBetweenBurstReduction,
                    minTimeBetweenBurst);
                
                //Set cooldown with added time for the burst.
                _burstCooldown = _currentTimeBetweenBurst + (shotOriginTransformMarkers.Length - 1) * burstInterval;
                
                //Remove burst from round.
                _burstsRemaining--;
            }
            
            //Fire charges that we have gained from loading a burst.
            if (_shotsRemainingInBurst > 0 && _intervalCooldown < 0f)
            {
                //Fire charge.
                var proj = Instantiate(shotPrefab, shotOriginTransformMarkers[^_shotsRemainingInBurst].transform);
                proj.transform.parent = null;
                if (!proj.TryGetComponent<IShot>(out var shot))
                    throw new Exception("Fired prefab doesn't have shot component.");
                //Provide info.
                shot.Origin = Slot.Entity;
                shot.Target = _target;
                //Reset charge interval.
                _intervalCooldown = burstInterval;
                _shotsRemainingInBurst--;
            }
        }
    }
}