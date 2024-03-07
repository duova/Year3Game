using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class MenuLoader : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        public HoverText hoverText;

        [SerializeField]
        private bool garage;

        private GameObject[] _currentObjects;

        public Dictionary<AddedEventsButton, GameObject> ButtonGameObjectRefPair { get; private set; } = new();

        public void UpdateCategory(GameObject[] gameObjects)
        {
            _currentObjects = gameObjects;
            
            foreach (var go in GetComponentsInChildren<Transform>().Select(t => t.gameObject))
            {
                if (go == gameObject) continue;
                Destroy(go);
            }
            
            ButtonGameObjectRefPair.Clear();

            foreach (var go in gameObjects)
            {
                bool isEntity = false;
                Sprite image = null;
                string text = null;
                if (go.TryGetComponent(out Entity.Entity entity))
                {
                    image = entity.image;
                    text = entity.text;
                    isEntity = true;
                }
                if (go.TryGetComponent(out Module module))
                {
                    image = module.image;
                    text = module.text;
                    isEntity = false;
                }

                var newGo = Instantiate(buttonPrefab, transform);
                var textComp = newGo.GetComponentInChildren<TMP_Text>();
                var imageComp = newGo.GetComponent<Image>();
                imageComp.sprite = image;
                var buttonComp = newGo.GetComponent<AddedEventsButton>();
                buttonComp.OnUp += OnClicked;
                buttonComp.OnEnter += OnEnter;
                buttonComp.OnExit += OnExit;
                
                ButtonGameObjectRefPair.Add(buttonComp, go);
            }
        }

        private void UpdateCategory()
        {
            UpdateCategory(_currentObjects);
        }

        private void OnClicked(AddedEventsButton buttonComp)
        {
            var refObject = ButtonGameObjectRefPair[buttonComp];
            if (!refObject) return;
            PlayerController.Instance.SelectObjectWithImage(refObject, buttonComp.GetComponent<Image>().sprite, garage);
            UpdateCategory();
        }
        
        private void OnEnter(AddedEventsButton buttonComp)
        {
            var refObject = ButtonGameObjectRefPair[buttonComp];
            if (!refObject) return;
            if (refObject.TryGetComponent<Entity.Entity>(out var entity))
            {
                hoverText.Activate(entity.text);
            }
            if (refObject.TryGetComponent<Module>(out var module))
            {
                hoverText.Activate(module.text);
            }
        }
        
        private void OnExit(AddedEventsButton buttonComp)
        {
            var refObject = ButtonGameObjectRefPair[buttonComp];
            if (!refObject) return;
            hoverText.Deactivate();
        }
    }
}