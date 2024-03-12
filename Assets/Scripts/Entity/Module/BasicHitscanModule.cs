using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Entity.Module
{
    public class BasicHitscanModule : Module
    {
        [SerializeField]
        private ModuleSlotType slotType;

        [SerializeField]
        private float damagePerSecond;
        
        [SerializeField]
        private float engagementRange;
        
        private bool _simulationTicker;

        private Entity _target;
        
        public override IEnumerable<ModuleSlotType> GetModuleTypes()
        {
            return new []{slotType};
        }

        public override bool IsAreaOfEffectAttack()
        {
            return false;
        }

        public override bool IsEngaged()
        {
            return Slot.Entity.IsInRange(_target, engagementRange);
        }

        private void FixedUpdate()
        {
            if (MatchManager.Instance.MatchState != MatchState.Simulation) return;
            
            //Only run on every other physics update to reduce lag from querying on tick.
            _simulationTicker = !_simulationTicker;
            
            if (_simulationTicker) return;

            _target = Slot.Entity.OrderedEnemyList[0];

            if (Slot.Entity.IsInRange(_target, engagementRange))
            {
                _target.AddHealth(-damagePerSecond / 30f);
            }
        }
    }
}