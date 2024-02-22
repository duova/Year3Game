using System;
using System.Linq;
using Core;
using Entity.Structure;
using Terrain;
using UnityEngine;

namespace UI
{
    public class SpawnMenu : MonoBehaviour
    {
        public static SpawnMenu Instance { get; private set; }

        public SpawnLocation SpawnLocation { get; private set; }

        public bool IsOpen { get; private set; }

        [SerializeField]
        private GameObject movableObject;

        [SerializeField]
        private Vector2 offset;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Open(SpawnLocation spawnLocation)
        {
            IsOpen = true;
            gameObject.SetActive(true);
            SpawnLocation = spawnLocation;
            Relocate((Vector2)Input.mousePosition - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + offset);
            spawnLocation.GetComponent<TileHighlight>().Override = true;
            if (spawnLocation.Node)
            {
                PlayerController.Instance.spawnMenuLoader.UpdateCategory(PlayerController.Instance.Actor.availableEntityPrefabs.Where(prefab => prefab.TryGetComponent<Drill>(out _)).ToArray());
            }
            else
            {
                PlayerController.Instance.spawnMenuLoader.UpdateCategory(PlayerController.Instance.Actor.availableEntityPrefabs.Where(prefab => !prefab.TryGetComponent<Drill>(out _)).ToArray());
            }
        }
        
        public void Close()
        {
            IsOpen = false;
            if (SpawnLocation != null) SpawnLocation.GetComponent<TileHighlight>().Override = false;
            SpawnLocation = null;
            PlayerController.Instance.spawnMenuLoader.hoverText.Deactivate();
            gameObject.SetActive(false);
        }

        private void Relocate(Vector2 pos)
        {
            movableObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}