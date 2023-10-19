using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class ScrollCategory : MonoBehaviour
    {
        [SerializeField]
        private float offset;

        private float currentOffset = 0;

        public Dictionary<PointerDownButton, GameObject> ButtonGameObjectRefPair { get; private set; } = new();

        public void UpdateCategory(GameObject[] gameObjects)
        {
            foreach (var go in GetComponentsInChildren<Transform>().Select(t => t.gameObject))
            {
                Destroy(go);
            }

            currentOffset = 0;
            ButtonGameObjectRefPair.Clear();

            foreach (var go in gameObjects)
            {
                bool isEntity;
                Sprite image = null;
                if (go.TryGetComponent(out Entity.Entity entity))
                {
                    image = entity.image;
                    isEntity = true;
                }
                if (go.TryGetComponent(out Module module))
                {
                    image = module.image;
                    isEntity = false;
                }

                var newGo = new GameObject();
                newGo.transform.parent = transform;
                var imageComp = newGo.AddComponent<Image>();
                imageComp.sprite = image;
                var buttonComp = newGo.AddComponent<PointerDownButton>();
                buttonComp.OnDown += OnClicked;
                newGo.transform.localPosition = new Vector3(0, currentOffset, 0);
                
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