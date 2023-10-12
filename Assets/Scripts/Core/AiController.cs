using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;
using UnityEngine;

namespace Core
{
    public class AiController : Controller
    {
        private List<Vector3> _unitPositions = new();
        
        private void FixedUpdate()
        {
            //Only act during strategy phase and don't revaluate if we're already ready.
            if (MatchManager.Instance.MatchState == MatchState.Simulation || MatchManager.Instance.ReadyActors.Contains(Actor)) return;
            
            //Cache opponent and self state data.
            
            //Analyse opponent and self unit buildup.
            //Get attack vector.
            //Get opponent and self power weighted positional average.
            //Locate weak targets accounting for attack direction and distance from defenses.
            
            //Prioritize building resource structures behind objectives.
            
            //Buildup units around objectives (but in front) for more defense.
            
            //Attack if unit power sum is above a certain threshold (range randomized to offer different levels of aggression).
            //Pull units back near objectives if we're not attacking.
            //Confront enemy unit group if confident, attack exposed objectives/buildings otherwise.
            
            //Ready
        }

        private SpawnLocation GetClosestEmptyOwnedSpawnLocation(Vector3 position)
        {
            var orderedLocations = Actor.SpawnLocations
                .OrderBy(spawnLoc => (spawnLoc.transform.position - position).sqrMagnitude)
                .ToArray();
            foreach (var loc in orderedLocations)
            {
                if (loc.Entity == null)
                {
                    return loc;
                }
            }
            return null;
        }
    }
}