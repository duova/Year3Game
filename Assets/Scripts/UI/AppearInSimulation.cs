using System;
using Core;
using UnityEngine;

namespace UI
{
    public class AppearInSimulation : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToChange;

        private void FixedUpdate()
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                objectToChange.SetActive(true);
            }
            else
            {
                objectToChange.SetActive(false);
            }
        }
    }
}