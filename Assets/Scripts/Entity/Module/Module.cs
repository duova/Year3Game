using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Entity.Module
{
    
    public abstract class Module : MonoBehaviour
    {
        public Sprite image;
        
        [TextArea(15,20)]
        public string text;
        
        public abstract IEnumerable<ModuleSlotType> GetModuleTypes();
        
        public abstract bool IsAreaOfEffectAttack();

        public ModuleSlot Slot { get; set; }

        public int price;

        public int InternalPowerRating => internalPowerRating;
        
        [SerializeField]
        private int internalPowerRating;

        [SerializeField]
        public int weight;

        public abstract bool IsEngaged();
    }
}