using System;
using System.Linq;
using Core;
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
    }
}