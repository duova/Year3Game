using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            foreach (var actor in MatchManager.Instance.Actors)
            {
                if (actor.Controller) continue;
                if (actor.Side == Side.Home)
                {
                    actor.Controller = actor.AddComponent<PlayerController>();
                }
                else
                {
                    actor.Controller = actor.AddComponent<AiController>();
                }
                actor.Controller.Actor = actor;
            }
        }
    }
}