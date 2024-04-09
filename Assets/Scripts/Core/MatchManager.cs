using System;
using System.Collections.Generic;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using TMPro;
using UI;
using UnityEngine;

namespace Core
{
    public enum MatchState
    {
        Strategy,
        Simulation
    }

    public enum TeamColor
    {
        Orange,
        Blue
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

        [SerializeField]
        private TMP_Text endText;

        public List<Actor> ReadyActors { get; set; } = new();

        [field: SerializeField]
        public Material OrangeMaterial { get; private set; }

        [field: SerializeField]
        public Material BlueMaterial { get; private set; }

        public TeamColor HomeColor { get; set; } = TeamColor.Orange;

        public TeamColor AwayColor { get; set; } = TeamColor.Blue;

        private void Awake()
        {
            Instance = this;
            _numActorsTotal = actors.Length;
        }

        private void Start()
        {
            BeginStrategy();
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
                BeginStrategy();
            }
        }

        private void BeginStrategy()
        {
            foreach (var actor in Actors)
            {
                actor.BeginStrategy();
            }
            RemainingStateTime = maxStratergyTime;
            MatchState = MatchState.Strategy;
            ReadyActors.Clear();
        }

        private void EndStrategy()
        {
            PlayerController.Instance.ClearSelectedUnits();
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
            
            if (PlayerController.Instance)
            {
                PlayerController.Instance.OnSimulationStart();
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
                    if (entity.GetType() != typeof(Objective) && !(entity.GetType() == typeof(Drill) && ((Drill)entity).roundsDisabled > 0))
                    {
                        entity.SetHealth(entity.MaxHealth);
                    }
                }
            }

            if (PlayerController.Instance)
            {
                PlayerController.Instance.OnSimulationEnd();
            }
            
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.ConditionalGoToSection(13, 14);
                TutorialManager.Instance.ConditionalGoToSection(19, 20);
            }
        }

        public void EndGame(bool won)
        {
            MatchState = MatchState.Strategy;
            endText.text = won ? "WIN" : "LOSE";
        }
    }
}