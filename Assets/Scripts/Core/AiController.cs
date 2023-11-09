using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Module;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    [Serializable]
    public struct RandomFloat
    {
        public float value;

        public float plusMinus;

        public float Get()
        {
            return value + Random.Range(-plusMinus, plusMinus);
        }
    }
    
    public class AiController : Controller
    {
        private List<Vector3> _unitPositions = new();

        [SerializeField]
        private RandomFloat maxPowerLevelDiffBeforeSpawningUnits;
        
        [SerializeField]
        private RandomFloat powerThresholdForAttack;
        
        [SerializeField]
        private RandomFloat powerLevelDiffToBeConfidentAtDirectConfrontation;

        [SerializeField]
        private float idealGapBetweenEntitiesToAvoidBlocking;
        
        [SerializeField]
        private float antiAoeDistanceFactor;

        private float _idealGapSquared;

        protected override void Awake()
        {
            base.Awake();
            _idealGapSquared = idealGapBetweenEntitiesToAvoidBlocking * idealGapBetweenEntitiesToAvoidBlocking;
        }

        private void FixedUpdate()
        {
            //Only act during strategy phase and don't revaluate if we're already ready.
            if (MatchManager.Instance.MatchState == MatchState.Simulation || MatchManager.Instance.ReadyActors.Contains(Actor)) return;
            
            //Cache opponent and self entities and state data.
            var selfEntities = Actor.Entities;
            var selfSpawnLocations = Actor.SpawnLocations;
            var opponents = MatchManager.Instance.Actors.Where(actor => actor != Actor);
            List<Entity.Entity> opponentEntities = new();
            foreach (var opponent in opponents)
            {
                opponentEntities.AddRange(opponent.Entities);
            }

            var selfPower = selfEntities.SelectMany( entity => entity.ModuleSlots ).Where(slot => slot.Module).Select( slot => slot.Module.InternalPowerRating ).Sum() + selfEntities.Select( entity => entity.InternalPowerRating).Sum();
            int opponentPower = 0;
            if (opponentEntities.Count > 0)
            {
                opponentPower =
                    opponentEntities.SelectMany(entity => entity.ModuleSlots)
                        .Where(slot => slot.Module).Select(slot => slot.Module.InternalPowerRating).Sum() +
                    opponentEntities.Select(entity => entity.InternalPowerRating).Sum();
            }

            //Analyse opponent and self unit buildup.
            //Get opponent and self power weighted positional average.
            var selfPowerWeightedAveragePosition = PowerWeightedAveragePosition(selfEntities);
            var opponentPowerWeightedAveragePosition = PowerWeightedAveragePosition(opponentEntities);
            //Calculate AoE percentage and grouping rating.
            var opponentModuleAoeIndicators = opponentEntities.SelectMany(entity => entity.ModuleSlots).Where(slot => slot.Module)
                .Select(slot => slot.Module.IsAreaOfEffectAttack()).ToArray();
            var opponentAoePercentage = (float)opponentModuleAoeIndicators.Count(indicator => indicator) /
                                        opponentModuleAoeIndicators.Length;
            var opponentPositionDifferences = opponentEntities.Select(entity =>
                (entity.transform.position - opponentPowerWeightedAveragePosition).sqrMagnitude).ToArray();
            var opponentGroupingRating = opponentPositionDifferences.Length > 0 ? opponentPositionDifferences.Average() : 0;
            //Calculate direct attack vector.
            var attackVector = (opponentPowerWeightedAveragePosition - selfPowerWeightedAveragePosition).normalized;

            //Locate a weak structure target accounting for distance from defenses and distance from our entities.
            var weakTarget = opponentEntities.Where( entity => !entity.GetType().IsSubclassOf(typeof(Unit)) && entity.GetType() != typeof(Unit)).OrderByDescending(entity =>
            {
                Vector3 position;
                return ((position = entity.transform.position) - opponentPowerWeightedAveragePosition).sqrMagnitude * 3 -
                       (position - selfPowerWeightedAveragePosition).sqrMagnitude;
            }).FirstOrDefault();
            
            while (true)
            {
                //Determine whether we want to spend on resource gathering or units.
                //Only spawn units if resource structures cannot be spawned or if a power difference threshold has been reached.
                var preferUnitPurchases = opponentPower - selfPower > maxPowerLevelDiffBeforeSpawningUnits.Get() || selfSpawnLocations.Where(location => location.Node).All(location => location.Entity);
                var boughtSomething = false;

                if (preferUnitPurchases)
                {
                    //Buy useful module if available.
                    var sortedModulePrefabs =
                        Actor.availableModulePrefabs.OrderByDescending(go =>
                            go.GetComponent<Module>().InternalPowerRating);
                    foreach (var prefab in sortedModulePrefabs)
                    {
                        if (Actor.PurchasedModulePrefabs.Contains(prefab)) continue;
                        if (prefab.GetComponent<Module>().price > Actor.currency) continue;
                        boughtSomething = Actor.PurchaseModule(prefab);
                        break;
                    }

                    //If we still have currency after buying module, build up units around center of power, with a min distance being the minimum + factor based on percentage of aoe attacks.
                    var availableEntities = Actor.availableEntityPrefabs.ToList();
                    availableEntities.RemoveAll(entity => entity.TryGetComponent<Drill>(out _));
                    while (availableEntities.Count > 0)
                    {
                        var entityToBuy = availableEntities[Random.Range(0, availableEntities.Count)];
                        if (entityToBuy.GetComponent<Entity.Entity>().price <= Actor.currency)
                        {
                            boughtSomething = Actor.PurchaseEntity(entityToBuy,
                                GetClosestEmptyOwnedSpawnLocation(selfPowerWeightedAveragePosition,
                                    _idealGapSquared + Mathf.Pow(antiAoeDistanceFactor * opponentAoePercentage, 2)));
                            break;
                        }
                        availableEntities.Remove(entityToBuy);
                    }
                }
                else
                {
                    //Build resource structure prioritizing nodes close to center of power.
                    var sortedEmptyNodeLocations = selfSpawnLocations.Where(location => location.Node && !location.Entity).OrderBy(location =>
                        (location.transform.position - selfPowerWeightedAveragePosition).sqrMagnitude);
                    var drillPrefab = Actor.availableEntityPrefabs.First(prefab => prefab.TryGetComponent<Drill>(out _));
                    if (drillPrefab.GetComponent<Drill>().price <= Actor.currency)
                    {
                        boughtSomething = Actor.PurchaseEntity(drillPrefab, sortedEmptyNodeLocations.First());
                    }
                }

                if (!boughtSomething) break;
            }
            
            //Equip units.
            foreach (var slot in Actor.Entities.SelectMany(entity => entity.ModuleSlots))
            {
                var nonAoeSelection = Actor.PurchasedModulePrefabs.Where(module =>
                {
                    var comp = module.GetComponent<Module>();
                    return !comp.IsAreaOfEffectAttack() && comp.GetModuleTypes().Contains(slot.slotType);
                });
                var aoeSelection = Actor.PurchasedModulePrefabs.Where(module =>
                {
                    var comp = module.GetComponent<Module>();
                    return comp.IsAreaOfEffectAttack() && comp.GetModuleTypes().Contains(slot.slotType);
                });
                if (Random.Range(0f, 100f) > opponentGroupingRating)
                {
                    //Uninstall
                    Actor.InstallModule(
                        nonAoeSelection.OrderByDescending(module => module.GetComponent<Module>().InternalPowerRating)
                            .First(), slot);
                }
                else
                {
                    Actor.InstallModule(
                        nonAoeSelection.OrderByDescending(module => module.GetComponent<Module>().InternalPowerRating)
                            .First(), slot);
                }
            }

            //Attack if unit power sum is above a certain threshold (range randomized to offer different levels of aggression).
            var doAttack = selfPower > powerThresholdForAttack.Get();
            
            //Confront enemy unit group if confident, attack exposed objectives/buildings otherwise.
            if (doAttack)
            {
                GameObject target;
                if (selfPower - opponentPower > powerLevelDiffToBeConfidentAtDirectConfrontation.Get())
                {
                    //Direct confrontation.
                    target = opponentEntities.OrderBy(opponentEntity =>
                            (opponentEntity.transform.position - opponentPowerWeightedAveragePosition).sqrMagnitude)
                        .First().gameObject;
                }
                else
                {
                    //Picking off structures.
                    target = weakTarget.gameObject;
                }
                
                foreach (var entity in Actor.Entities.Where(entity => entity.GetType() == typeof(Unit) || entity.GetType().IsSubclassOf(typeof(Unit))))
                {
                    entity.Actor.GiveOrder(OrderType.Move, target, (Unit)entity);
                }
            }

            //Ready
            Actor.Ready();
        }

        private SpawnLocation GetClosestEmptyOwnedSpawnLocation(Vector3 position, float minDistanceSquaredFromAnotherEntity)
        {
            var allEntities = MatchManager.Instance.Actors.SelectMany(actor => actor.Entities).ToArray();
            var orderedLocations = Actor.SpawnLocations
                .OrderBy(spawnLoc => (spawnLoc.transform.position - position).sqrMagnitude)
                .ToArray();
            return orderedLocations.FirstOrDefault(loc => loc.Entity == null && !allEntities.Any(entity => (entity.transform.position - position).sqrMagnitude > minDistanceSquaredFromAnotherEntity));
        }

        private static Vector3 PowerWeightedAveragePosition(IEnumerable<Entity.Entity> entities)
        {
            var vectorSum = Vector3.zero;
            var denominator = 0;
            foreach (var entity in entities)
            {
                var power = entity.InternalPowerRating +
                            entity.ModuleSlots.Where(slot =>slot.Module).Select(slot => slot.Module.InternalPowerRating).Sum();
                denominator += power;
                vectorSum += entity.transform.position * power;
            }
            if (denominator == 0) return Vector3.zero;
            return vectorSum / denominator;
        }
    }
}