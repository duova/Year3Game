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

        public UnitOrder(OrderType orderType, GameObject target)
        {
            OrderType = orderType;
            Target = target;
        }
    }
    
    public class Unit : Entity
    {
        public UnitOrder Order { get; set; }

        public GameObject Target { get; private set; }

        private NavMeshAgent _agent;

        private bool _inEnemyTerritory;

        [SerializeField]
        private float engagementRange;

        [SerializeField]
        private float rotateSpeed = 360;

        protected override void Awake()
        {
            base.Awake();
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

            if (!MatchManager.Instance.ActiveUnits.Contains(this))
            {
                MatchManager.Instance.ActiveUnits.Add(this);
            }
        }

        public override void EndSimulation()
        {
            Target = null;
            Order = default;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (SimulationTicker) return;

            if (MatchManager.Instance.MatchState == MatchState.Strategy) return;
            
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
            
            //Rotate towards target.
            if (Target)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(Target.transform.position - transform.position, Vector3.up), rotateSpeed / 30f);
            }

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