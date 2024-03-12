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

        private void OnMouseEnter()
        {
            if (SpawnMenu.Instance.SpawnLocation != null) return;
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            if (GetComponent<SpawnLocation>().Actor == PlayerController.Instance.Actor || PlayerController.Instance.Selected != null || GetComponent<SpawnLocation>().Entity && GetComponent<SpawnLocation>().Entity.Actor == PlayerController.Instance.Actor)
            {
                displayObject.SetActive(true);
            }
        }

        private void OnMouseExit()
        {
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            if (_override) return;
            displayObject.SetActive(false);
        }
    }
}