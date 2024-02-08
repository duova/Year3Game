using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class ScrollCategory : MonoBehaviour
    {
        [SerializeField]
        private GameObject scrollButtonPrefab;

        public Dictionary<PointerDownButton, GameObject> ButtonGameObjectRefPair { get; private set; } = new();

        public void UpdateCategory(GameObject[] gameObjects)
        {
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

                var newGo = Instantiate(scrollButtonPrefab, transform);
                var textComp = newGo.GetComponentInChildren<TMP_Text>();
                textComp.text = text;
                var imageComp = newGo.GetComponent<Image>();
                imageComp.sprite = image;
                var buttonComp = newGo.GetComponent<PointerDownButton>();
                buttonComp.OnDown += OnClicked;
                if (!isEntity && !PlayerController.Instance.Actor.PurchasedModulePrefabs.Contains(go))
                {
                    imageComp.color = new Color(imageComp.color.r, imageComp.color.g, imageComp.color.b, 0.5f);
                }
                
                ButtonGameObjectRefPair.Add(buttonComp, go);
            }
        }

        private void OnClicked(PointerDownButton buttonComp)
        {
            var refObject = ButtonGameObjectRefPair[buttonComp];
            if (!refObject) return;
            PlayerController.Instance.SelectObjectWithImage(refObject, buttonComp.GetComponent<Image>().sprite);
        }
    }
}