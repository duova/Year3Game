using System;
using Core;
using UnityEngine;

namespace UI
{
    public class TileHighlight : MonoBehaviour
    {
        [SerializeField]
        private GameObject displayObject;

        private void Start()
        {
            displayObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                displayObject.SetActive(false);
            }
        }

        private void OnMouseEnter()
        {
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            displayObject.SetActive(true);
        }

        private void OnMouseExit()
        {
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            displayObject.SetActive(false);
        }
    }
}