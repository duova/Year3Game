using System;
using System.Collections.Generic;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using UnityEngine;

namespace Core
{
    public enum MatchState
    {
        Strategy,
        Simulation
    }
    
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance { get; private set; }

        public Actor[] Actors => actors;
        
        [SerializeField]
        private Actor[] actors;

        public MatchState MatchState { get; private set; }

        public float RemainingStateTime { get; private set; }
        
        public List<Unit> ActiveUnits { get; set; } = new();

        [SerializeField]
        private float maxStratergyTime;

        [SerializeField]
        private float maxSimulationTime;

        private int _numActorsTotal;

        public List<Actor> ReadyActors { get; set; } = new();

        private void Awake()
        {
            Instance = this;
            _numActorsTotal = actors.Length;
        }

        private void Start()
        {
            BeginStratergy();
        }

        private void Update()
        {
            RemainingStateTime -= Time.deltaTime;
            if (MatchState == MatchState.Strategy)
            {
                //Only stop strategy if out of time or all actors ready.
                if (RemainingStateTime >= 0 && ReadyActors.Count < _numActorsTotal) return;
                EndStrategy();
                BeginSimulation();
            }
            else
            {
                //Only stop simulation if out of time or no more units are active.
                if (RemainingStateTime >= 0 && ActiveUnits.Count > 0) return;
                EndSimulation();
                BeginStratergy();
            }
        }

        private void BeginStratergy()
        {
            RemainingStateTime = maxStratergyTime;
            MatchState = MatchState.Strategy;
            ReadyActors.Clear();
        }

        private void EndStrategy()
        {
            
        }

        private void BeginSimulation()
        {
            RemainingStateTime = maxSimulationTime;
            MatchState = MatchState.Simulation;
            foreach (var actor in Actors)
            {
                foreach (var entity in actor.Entities)
                {
                    entity.BeginSimulation();
                }
            }
        }

        private void EndSimulation()
        {
            ActiveUnits.Clear();
            foreach (var actor in Actors)
            {
                foreach (var entity in actor.Entities)
                {
                    entity.EndSimulation();
                    //Heal all entities after simulation.
                    if (entity.GetType() != typeof(Objective))
                    {
                        entity.SetHealth(entity.MaxHealth);
                    }
                }
            }
        }
    }
}