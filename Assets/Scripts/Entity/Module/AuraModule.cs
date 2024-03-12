using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Entity.Module
{
    public class AuraModule : Module
    {
        public const float DamageReduction = 0.3f;

        //Most logic here is done by the entity class instead to query on damage.
        [SerializeField]
        private ModuleSlotType slotType;

        [SerializeField]
        public float rangeSquared;
        
        private bool _simulationTicker;
        
        public override IEnumerable<ModuleSlotType> GetModuleTypes()
        {
            return new[] { slotType };
        }

        public override bool IsAreaOfEffectAttack()
        {
            return false;
        }

        public override bool IsEngaged()
        {
            return false;
        }
    }
}