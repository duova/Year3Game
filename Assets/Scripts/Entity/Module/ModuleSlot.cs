using System;
using System.Linq;
using UnityEngine;

namespace Entity.Module
{
    public enum ModuleSlotType
    {
        Any,
        SmallWeapon,
        LargeWeapon
    }
    
    public class ModuleSlot : MonoBehaviour
    {
        public Module Module { get; private set; }

        public Entity Entity { get; set; }

        public ModuleSlotType slotType;

        public bool InstallModule(GameObject modulePrefab)
        {
            if (Module) return false;
            if (!modulePrefab.TryGetComponent(out Module module))
            {
                throw new Exception("Tried to install module object that has no module component.");
            }
            if (!module.GetModuleTypes().Contains(slotType)) return false;
            Module = Instantiate(modulePrefab, transform).GetComponent<Module>();
            Module.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Module.Slot = this;
            return true;
        }

        public void UninstallModule()
        {
            if (!Module) return;
            Destroy(Module.gameObject);
            Module = null;
        }
    }
}