using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using Terrain;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Unit
{
    public enum OrderType
    {
        Stay,
        Move, //Target should be an object with a SpawnLocation component.
        Follow //Target should be a unit.
    }
    
    public struct UnitOrder
    {
        public OrderType OrderType;
        public GameObject Target;
    }
    
    public class Unit : Entity
    {
        public UnitOrder Order { get; set; }

        public GameObject Target { get; private set; }

        public ModuleSlot[] ModuleSlots { get; private set; }

        private NavMeshAgent _agent;

        private bool _inEnemyTerritory;

        private bool _simulating;

        [SerializeField]
        private float engagementRange;

        private bool _simulationTicker;

        public List<Entity> OrderedEnemyList { get; private set; }

        private void Awake()
        {
            ModuleSlots = GetComponentsInChildren<ModuleSlot>();
            foreach (var slot in ModuleSlots)
            {
                slot.Unit = this;
            }

            if (!TryGetComponent(out _agent))
            {
                throw new Exception("Unit does not have NavMeshAgent.");
            }
        }

        public override void BeginSimulation()
        {
            if (Order.OrderType is OrderType.Move or OrderType.Follow)
            {
                Target = Order.Target;
            }
            else
            {
                Target = null;
            }

            _simulating = true;

            if (!MatchManager.Instance.ActiveUnits.Contains(this))
            {
                MatchManager.Instance.ActiveUnits.Add(this);
            }
        }

        public override void EndSimulation()
        {
            Target = null;
            _simulating = false;
            Order = default;
        }

        private void FixedUpdate()
        {
            //Only run on every other physics update to reduce lag from querying on tick.
            _simulationTicker = !_simulationTicker;
            
            if (!_simulating || _simulationTicker) return;

            //Determine if closest spawn location is owned by the enemy.
            List<SpawnLocation> spawnLocations = new();
            foreach (var actor in MatchManager.Instance.Actors)
            {
                spawnLocations.AddRange(actor.SpawnLocations);
            }
            var orderedLocations = spawnLocations.OrderBy(location => (location.transform.position - transform.position).sqrMagnitude).ToArray();
            _inEnemyTerritory = orderedLocations[0].Actor != Actor;
            
            //Targeting.
            foreach (var actor in MatchManager.Instance.Actors)
            {
                if (actor == Actor) continue;
                if (actor.Entities.Count == 0) continue;
                OrderedEnemyList = actor.Entities.OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList();
                //Only engage if in engagement range or within enemy territory, in which case we need the unit to keep trying to engage.
                if (_inEnemyTerritory || IsInEngagementRange(OrderedEnemyList[0]))
                {
                    Target = OrderedEnemyList[0].gameObject;
                }
                else
                {
                    Target = null;
                }
            }

            _agent.destination = Target ? Target.transform.position : transform.position;

            //Only allow the simulation phase to end if this unit is not in enemy territory and does not have a target.
            if (_inEnemyTerritory || Target) return;
            if (MatchManager.Instance.ActiveUnits.Contains(this))
            {
                MatchManager.Instance.ActiveUnits.Remove(this);
            }
        }

        private bool IsInEngagementRange(Entity target)
        {
            return IsInRange(target, engagementRange);
        }
        
        public bool IsInRange(Entity target, float range)
        {
            return (target.transform.position - transform.position).sqrMagnitude <
                   range * range;
        }

        public override void Destroy()
        {
            //Remove from the tracker that waits for units to complete their actions.
            if (MatchManager.Instance.ActiveUnits.Contains(this))
            {
                MatchManager.Instance.ActiveUnits.Remove(this);
            }
            base.Destroy();
        }
    }
}