using System.Collections.Generic;
using UnityEngine;

namespace Entity.Module
{
    
    public abstract class Module : MonoBehaviour
    {
        public abstract IEnumerable<ModuleSlotType> GetModuleTypes();

        public ModuleSlot Slot { get; set; }

        public int price;
    }
}