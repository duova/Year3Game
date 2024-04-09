using System;
using Core;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI
{
    public class TileHighlight : MonoBehaviour
    {
        [SerializeField]
        private GameObject displayObject;

        [SerializeField] private bool _override;

        public bool Override
        {
            get => _override;
            set
            {
                _override = value;
                displayObject.SetActive(value);
            }
        }

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

        public void OnMouseEnter()
        {
            if (SpawnMenu.Instance.SpawnLocation != null) return;
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            displayObject.SetActive(true);
        }

        public void OnMouseExit()
        {
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            if (_override) return;
            displayObject.SetActive(false);
        }
    }
}