using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Module;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using UnityEngine;

namespace Core
{
    public enum Side
    {
        Home,
        Away
    }
    
    public class Actor : MonoBehaviour
    {
        //Controllers are created by the game manager upon loading a scene.
        public Controller Controller { get; set; }

        public List<SpawnLocation> SpawnLocations { get; private set; } = new();

        public List<Entity.Entity> Entities { get; set; } = new();

        public Side Side => side;

        [SerializeField]
        private Side side;
        
        public int currency;

        public GameObject[] availableModulePrefabs;

        public GameObject[] availableEntityPrefabs;

        public List<GameObject> PurchasedModulePrefabs { get; set; } = new();
        
        public List<Objective> Objectives { get; set; } = new();

        [SerializeField]
        private SpawnLocation[] objectiveLocations;
        
        [SerializeField]
        private GameObject objectivePrefab;

        private void Awake()
        {
            foreach (var location in objectiveLocations)
            {
                Objectives.Add(location.SpawnEntity(objectivePrefab).GetComponent<Objective>());
            }
            SpawnLocations = FindObjectsOfType<SpawnLocation>().ToList();
            SpawnLocations.RemoveAll(location => location.Side != Side);
            foreach (var location in SpawnLocations)
            {
                location.Actor = this;
            }
        }

        #region Controller Callable Actions
        
        public bool PurchaseEntity(GameObject prefab, SpawnLocation location)
        {
            if (!prefab || !location) return false;
            if (location.Actor != this) return false;
            if (!prefab.TryGetComponent(out Entity.Entity prefabEntity))
            {
                throw new Exception("Tried to purchase entity GameObject that doesn't have an entity component.");
            }
            if (prefabEntity.price > currency) return false;
            if (prefabEntity.GetType() == typeof(Drill) && !location.Node) return false;
            if (!location.SpawnEntity(prefab)) return false;
            currency -= prefabEntity.price;
            return true;
        }

        public bool PurchaseModule(GameObject modulePrefab)
        {
            if (!modulePrefab) return false;
            if (!modulePrefab.TryGetComponent(out Module module))
            {
                throw new Exception("Tried to purchase module GameObject that doesn't have a module component.");
            }
            if (module.price > currency) return false;
            if (!availableModulePrefabs.Contains(modulePrefab)) return false;
            if (PurchasedModulePrefabs.Contains(modulePrefab)) return false;
            PurchasedModulePrefabs.Add(modulePrefab);
            currency -= module.price;
            return true;
        }

        public bool InstallModule(GameObject modulePrefab, ModuleSlot slot)
        {
            if (!modulePrefab || !PurchasedModulePrefabs.Contains(modulePrefab)) return false;
            return slot.InstallModule(modulePrefab);
        }
        
        public void UninstallModule(ModuleSlot slot)
        { 
            slot.UninstallModule();
        }

        public void Ready()
        {
            if (MatchManager.Instance.ReadyActors.Contains(this)) return;
            MatchManager.Instance.ReadyActors.Add(this);
        }

        public bool GiveOrder(OrderType orderType, GameObject target, Unit unit)
        {
            if (!unit || !target) return false;
            if (orderType == OrderType.Move && !target.TryGetComponent<SpawnLocation>(out _)) return false;
            if (orderType == OrderType.Follow && !target.TryGetComponent<Unit>(out _)) return false;
            unit.Order = new UnitOrder { OrderType = orderType, Target = target };
            return true;
        }
        
        #endregion

        public void CheckIfLost()
        {
            if (Objectives.Count == 0)
            {
                print(Side == Side.Home ? "Away wins." : "Home wins.");
            }
        }
    }
}