using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Entity.Module
{
    
    public abstract class Module : MonoBehaviour
    {
        public Sprite image;
        
        public abstract IEnumerable<ModuleSlotType> GetModuleTypes();
        
        public abstract bool IsAreaOfEffectAttack();

        public ModuleSlot Slot { get; set; }

        public int price;

        public int InternalPowerRating => internalPowerRating;
        
        [SerializeField]
        private int internalPowerRating;
    }
}