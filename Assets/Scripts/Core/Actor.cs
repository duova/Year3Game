using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Module;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

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

        [SerializeField]
        private int currency;

        [SerializeField]
        public int maxCurrency = 100;

        [SerializeField]
        private GameObject placeVFXPrefab;

        public int Currency
        {
            get => currency;
            set
            {
                if (PlayerController.Instance.Actor == this && value < currency)
                {
                    DeductCurrencyText.Instance.Deduct(currency - value);
                }

                currency = Mathf.Min(value, maxCurrency);
            }
        }

        public GameObject[] availableModulePrefabs;

        public GameObject[] availableEntityPrefabs;

        public List<GameObject> PurchasedModulePrefabs { get; set; } = new();
        
        public List<Objective> Objectives { get; set; } = new();

        [SerializeField]
        private SpawnLocation[] objectiveLocations;
        
        [SerializeField]
        private GameObject objectivePrefab;
        
        [SerializeField]
        private GameObject drillPrefab;

        [FormerlySerializedAs("defaultOrderColor")] [SerializeField]
        private Material defaultOrderMat;
        
        [FormerlySerializedAs("followAllyOrderColor")] [SerializeField]
        private Material followAllyOrderMat;
        
        [FormerlySerializedAs("followEnemyOrderColor")] [SerializeField]
        private Material followEnemyOrderMat;

        private void Awake()
        {
            SpawnLocations = FindObjectsOfType<SpawnLocation>().ToList();
            SpawnLocations.RemoveAll(location => location.Side != Side);
            foreach (var location in SpawnLocations)
            {
                location.Actor = this;
            }
            foreach (var location in objectiveLocations)
            {
                Objectives.Add(location.SpawnEntity(objectivePrefab).GetComponent<Objective>());
            }
        }

        public void ProcessUpkeep(int amount)
        {
            currency -= amount;
            if (PlayerController.Instance.Actor == this)
            {
                PlayerController.Instance.upkeepThisRound += amount;
            }
        }

        private void Start()
        {
            foreach (var location in SpawnLocations)
            {
                if (location.Node)
                {
                    location.SpawnEntity(drillPrefab);
                }
            }
        }

        public void BeginStrategy()
        {
            currency = Mathf.Max(currency, 0);
        }

        #region Controller Callable Actions
        
        public bool PurchaseEntity(GameObject prefab, SpawnLocation location, bool pay = true)
        {
            if (!prefab || !location) return false;
            if (location.Actor != this) return false;
            if (!prefab.TryGetComponent(out Entity.Entity prefabEntity))
            {
                throw new Exception("Tried to purchase entity GameObject that doesn't have an entity component.");
            }
            if (pay && prefabEntity.price > Currency) return false;
            if (prefabEntity.GetType() == typeof(Drill) && !location.Node)
            {
                print("Drill can only be placed on a node.");
                return false;
            }
            if (!location.SpawnEntity(prefab)) return false;
            if (pay)
            {
                Currency -= prefabEntity.price;
            }

            if (!GarageManager.Instance.inGarage)
            {
                Destroy(Instantiate(placeVFXPrefab, location.Entity.transform.position, Quaternion.identity), 2f);
            }

            return true;
        }

        public bool InstallModule(GameObject modulePrefab, ModuleSlot slot, bool pay = true)
        {
            if (!modulePrefab/* || !PurchasedModulePrefabs.Contains(modulePrefab) */) return false;
            if (!modulePrefab.TryGetComponent(out Module module))
            {
                throw new Exception("Tried to purchase module GameObject that doesn't have a module component.");
            }

            if (pay)
            {
                if (module.price > Currency) return false;
                Currency -= module.price;
            }

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

        public bool GiveOrder(OrderType orderType, GameObject target, Unit unit, bool drawLine)
        {
            Entity.Entity targetEntity = null;
            var material = defaultOrderMat;
            if (!unit || !target) return false;
            if (orderType == OrderType.Move && !target.TryGetComponent<SpawnLocation>(out _)) return false;
            if (orderType == OrderType.Follow && !target.TryGetComponent(out targetEntity)) return false;
            if (orderType == OrderType.Follow && targetEntity == unit) orderType = OrderType.Stay;
            if (orderType == OrderType.Stay) target = null;
            unit.Order = new UnitOrder { OrderType = orderType, Target = target };
            if (drawLine)
            {
                unit.LineRenderer.positionCount = 2;
                unit.LineRenderer.SetPositions(new[] { unit.transform.position, target ? target.transform.position : unit.transform.position });
                if (orderType == OrderType.Follow)
                {
                    if (targetEntity && targetEntity.Actor == this)
                    {
                        material = followAllyOrderMat;
                    }
                    else
                    {
                        material = followEnemyOrderMat;
                    }
                }

                unit.LineRenderer.material = material;
            }
            return true;
        }
        
        #endregion

        public void CheckIfLost()
        {
            if (Objectives.Count == 0)
            {
                MatchManager.Instance.EndGame(Side != Side.Home);
            }
        }
    }
}