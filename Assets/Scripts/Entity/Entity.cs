using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using Terrain;
using UnityEngine;
using UnityEngine.AI;

namespace Entity
{
    public abstract class Entity : MonoBehaviour
    {
        public Actor Actor { get; set; }

        [field: SerializeField]
        public SpawnLocation SpawnLocation { get; set; }

        public int price;
        
        public Sprite image;

        [TextArea(15,20)]
        public string text;

        public float MaxHealth => maxHealth;

        [SerializeField]
        private float maxHealth;
        
        public float Health { get; private set; }
        
        public ModuleSlot[] ModuleSlots { get; private set; }

        public List<Entity> OrderedEnemyList { get; private set; } = new();

        protected bool SimulationTicker;

        public int InternalPowerRating => internalPowerRating;
        
        [SerializeField]
        private int internalPowerRating;
        
        [SerializeField]
        private GameObject healthBar;

        private float _initialHealthBarXScale;

        [SerializeField]
        private GameObject moduleArrowPrefab;

        private GameObject moduleArrow;

        protected virtual void Awake()
        {
            ModuleSlots = GetComponentsInChildren<ModuleSlot>();
            foreach (var slot in ModuleSlots)
            {
                slot.Entity = this;
            }
            
            _initialHealthBarXScale = healthBar.transform.localScale.x;
        }

        protected virtual void Start()
        {
            SetHealth(maxHealth);
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
            if (healthChange < 0)
            {
                //Check for aura.
                var orderedAllyList = Actor.Entities.OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList();
                foreach (var ally in orderedAllyList.Where(ally => ally != this))
                {
                    if (!ally.ModuleSlots.FirstOrDefault(slot => slot.Module != null
                                                                 && slot.Module is AuraModule auraModule
                                                                 && (ally.transform.position - transform.position).sqrMagnitude <
                                                                 auraModule.rangeSquared)) continue;
                    //Reduce damage.
                    healthChange *= 1 - AuraModule.DamageReduction;
                    break;
                }
            }
            SetHealth(Health + healthChange);
        }

        public void AttachToClosest()
        {
            //Find closest spawn location that can accomodate.
            var orderedLocations = MatchManager.Instance.Actors.SelectMany(actor => actor.SpawnLocations).OrderBy(location => (location.transform.position - transform.position).sqrMagnitude);
            foreach (var location in orderedLocations)
            {
                //Simply reset if the tile is the one it was on last turn.
                if (location.Entity == this)
                {
                    transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    GetComponent<NavMeshAgent>().destination = transform.position;
                    return;
                }
                if (location.Entity) continue;
                Detach();
                location.SetEntity(this);
                GetComponent<NavMeshAgent>().destination = transform.position;
                return;
            }
            throw new Exception("No valid location to attach to.");
        }

        public void Detach()
        {
            if (!SpawnLocation) return;
            SpawnLocation.DetachEntity();
        }

        public virtual void Destroy()
        {
            Actor.Entities.Remove(this);
            Detach();
            Destroy(gameObject);
        }

        public void HideHealth()
        {
            healthBar.SetActive(false);
        }

        public abstract void BeginSimulation();

        public abstract void EndSimulation();
        
        public bool IsInRange(Entity target, float range)
        {
            if (!target) return false;
            return (target.transform.position - transform.position).sqrMagnitude <
                   range * range;
        }

        protected virtual void FixedUpdate()
        {
            //Only run on every other physics update to reduce lag from querying on tick.
            SimulationTicker = !SimulationTicker;
            
            if (SimulationTicker) return;

            if (GarageManager.Instance.inGarage) return;
            
            foreach (var actor in MatchManager.Instance.Actors)
            {
                if (actor == Actor) continue;
                if (actor.Entities.Count == 0) continue;
                OrderedEnemyList = actor.Entities
                    .OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList();
            }

            var healthLocalScale = healthBar.transform.localScale;
            healthLocalScale = new Vector3(_initialHealthBarXScale * (Health / maxHealth), healthLocalScale.y, healthLocalScale.z);
            healthBar.transform.localScale = healthLocalScale;

            /*
            if (moduleArrowPrefab != null && Actor == PlayerController.Instance.Actor)
            {
                if (MatchManager.Instance.MatchState == MatchState.Simulation)
                {
                    if (moduleArrow != null)
                    {
                        Destroy(moduleArrow);
                    }
                }
                else
                {
                    if (moduleArrow != null)
                    {
                        if (ModuleSlots.All(slot => slot.Module != null))
                        {
                            Destroy(moduleArrow);
                        }
                    }
                    else
                    {
                        if (ModuleSlots.Any(slot => slot.Module == null))
                        {
                            moduleArrow = Instantiate(moduleArrowPrefab, transform);
                        }
                    }
                }
            }
            */
        }
    }
}