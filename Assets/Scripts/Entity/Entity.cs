using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using Terrain;
using UnityEngine;

namespace Entity
{
    public abstract class Entity : MonoBehaviour
    {
        public Actor Actor { get; set; }

        public SpawnLocation SpawnLocation { get; set; }

        public int price;

        public float MaxHealth => maxHealth;

        [SerializeField]
        private float maxHealth;
        
        public float Health { get; private set; }
        
        public ModuleSlot[] ModuleSlots { get; private set; }
        
        public List<Entity> OrderedEnemyList { get; private set; }

        protected bool SimulationTicker;

        protected virtual void Awake()
        {
            ModuleSlots = GetComponentsInChildren<ModuleSlot>();
            foreach (var slot in ModuleSlots)
            {
                slot.Entity = this;
            }
        }

        public void SetHealth(float newHealth)
        {
            if (newHealth > MaxHealth)
            {
                newHealth = MaxHealth;
            }
            Health = newHealth;
            if (Health > 0f) return;
            //Play destroy effects.
            Destroy();
        }

        public void AddHealth(float healthChange)
        {
            SetHealth(Health + healthChange);
        }

        public void AttachToClosest()
        {
            //Find closest spawn location that can accomodate.
            var orderedLocations = Actor.SpawnLocations.OrderBy(location => (location.transform.position - transform.position).sqrMagnitude);
            foreach (var location in orderedLocations)
            {
                if (location.Entity) continue;
                location.SetEntity(this);
            }
            throw new Exception("No valid location to attach to.");
        }

        public void Detach()
        {
            if (!SpawnLocation) return;
            SpawnLocation.DetachEntity(this);
        }

        public virtual void Destroy()
        {
            Actor.Entities.Remove(this);
            Detach();
            Destroy(gameObject);
        }

        public abstract void BeginSimulation();

        public abstract void EndSimulation();
        
        public bool IsInRange(Entity target, float range)
        {
            return (target.transform.position - transform.position).sqrMagnitude <
                   range * range;
        }

        protected virtual void FixedUpdate()
        {
            //Only run on every other physics update to reduce lag from querying on tick.
            SimulationTicker = !SimulationTicker;
            
            if (SimulationTicker) return;
            
            foreach (var actor in MatchManager.Instance.Actors)
            {
                if (actor == Actor) continue;
                if (actor.Entities.Count == 0) continue;
                OrderedEnemyList = actor.Entities.OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList();
            }
        }
    }
}