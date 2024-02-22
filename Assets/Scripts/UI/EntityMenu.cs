using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entity.Module;
using Entity.Structure;
using Terrain;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unit = Entity.Unit.Unit;

namespace UI
{
    public class EntityMenu : MonoBehaviour
    {
        public static EntityMenu Instance { get; private set; }

        public Entity.Entity Entity { get; private set; }

        public bool IsOpen { get; private set; }

        [SerializeField]
        private GameObject movableObject;

        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private TMP_Text infoText;

        [SerializeField]
        private GameObject gridObject;
        
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private Sprite plusSprite;

        [SerializeField]
        private ModuleMenu moduleMenu;
        
        public Dictionary<AddedEventsButton, ModuleSlot> ButtonSlotRefPair { get; private set; } = new();

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Open(Entity.Entity entity)
        {
            IsOpen = true;
            gameObject.SetActive(true);
            Entity = entity;
            Relocate((Vector2)Input.mousePosition - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + offset);
            infoText.text = entity.text;
            PresentSlots();
        }
        
        public void Close()
        {
            IsOpen = false;
            Entity = null;
            moduleMenu.Close();
            gameObject.SetActive(false);
        }

        private void Relocate(Vector2 pos)
        {
            movableObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }
        
        public void PresentSlots()
        {
            var slots = Entity.ModuleSlots;
            
            foreach (var go in gridObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject))
            {
                if (go == gridObject.gameObject) continue;
                Destroy(go);
            }
            
            ButtonSlotRefPair.Clear();

            foreach (var slot in slots)
            {
                var image = slot.Module ? slot.Module.image : plusSprite;
                
                var newGo = Instantiate(buttonPrefab, gridObject.transform);
                var textComp = newGo.GetComponentInChildren<TMP_Text>();
                var imageComp = newGo.GetComponent<Image>();
                imageComp.sprite = image;
                var buttonComp = newGo.GetComponent<AddedEventsButton>();
                
                buttonComp.OnUp += OnClicked;
                buttonComp.OnEnter += OnEnter;
                buttonComp.OnExit += OnExit;
                
                ButtonSlotRefPair.Add(buttonComp, slot);
            }
        }
        
        private void OnClicked(AddedEventsButton buttonComp)
        {
            var slot = ButtonSlotRefPair[buttonComp];
            if (!slot) return;
            moduleMenu.Open(slot);
        }
        
        private void OnEnter(AddedEventsButton buttonComp)
        {
        }
        
        private void OnExit(AddedEventsButton buttonComp)
        {
        }
    }
}