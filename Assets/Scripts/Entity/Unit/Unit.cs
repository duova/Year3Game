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
    
    [Serializable]
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
    
    [RequireComponent(typeof(LineRenderer))]
    public class Unit : Entity
    {
        [field: SerializeField]
        public UnitOrder Order { get; set; }

        public GameObject Target { get; private set; }

        private NavMeshAgent _agent;

        private bool _inEnemyTerritory;

        [SerializeField]
        private float engagementRange;

        [SerializeField]
        private float rotateSpeed = 360;

        private float _originalSpeed;

        [HideInInspector]
        public LineRenderer LineRenderer;

        protected override void Awake()
        {
            base.Awake();
            if (!TryGetComponent(out _agent))
            {
                throw new Exception("Unit does not have NavMeshAgent.");
            }

            _originalSpeed = _agent.speed;

            LineRenderer = GetComponent<LineRenderer>();
        }

        public override void BeginSimulation()
        {
            LineRenderer.positionCount = 0;
            
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
            
            _agent.speed = _originalSpeed;
            foreach (var slot in ModuleSlots)
            {
                if (slot.Module == null) continue;
                _agent.speed -= slot.Module.weight;
            }
        }

        public override void EndSimulation()
        {
            Target = null;
            Order = default;
            AttachToClosest();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (SimulationTicker) return;
            
            _agent.destination = Target ? Target.transform.position : transform.position;

            if (MatchManager.Instance.MatchState == MatchState.Strategy) return;

            //If the unit regains a target, we want to add it to the list again.
            if (Target)
            {
                if (!MatchManager.Instance.ActiveUnits.Contains(this))
                {
                    MatchManager.Instance.ActiveUnits.Add(this);
                }
            }
            
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
            }
            
            //Clear target if it is a spawn location and the unit has arrived.
            if (Target && Target.TryGetComponent<SpawnLocation>(out _) && (Target.transform.position - transform.position).sqrMagnitude <= 0.5f * 0.5f) Target = null;
            
            //Rotate towards target.
            if (Target)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(Target.transform.position - transform.position, Vector3.up), rotateSpeed / 30f);
            }
            
            //Always in combat if in enemy territory.
            if (_inEnemyTerritory) return;
            //If target isn't null we have to check if it can still path.
            if (Target != null)
            {
                //Move orders can generally always reach.
                if (Order.OrderType == OrderType.Move) return;
                if (!Target.TryGetComponent(out Entity targetEntity)) return;
                //If we're following an enemy we want to continue.
                if (targetEntity.Actor != Actor) return;
                //We only want to follow allies that are still active.
                if (MatchManager.Instance.ActiveUnits.Contains(targetEntity)) return;
                //Or are really far away.
                if ((Target.transform.position - transform.position).sqrMagnitude > 1f * 1f) return;
            }
            //Remove it from the active list.
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