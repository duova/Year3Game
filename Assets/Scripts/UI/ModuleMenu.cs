using System;
using System.Linq;
using Core;
using Entity.Module;
using Entity.Structure;
using Terrain;
using UnityEngine;

namespace UI
{
    public class ModuleMenu : MonoBehaviour
    {
        public static ModuleMenu Instance { get; private set; }

        public ModuleSlot ModuleSlot { get; private set; }

        public bool IsOpen { get; private set; }

        [SerializeField]
        private GameObject movableObject;

        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private MenuLoader moduleMenuLoader;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Open(ModuleSlot slot)
        {
            return;
            IsOpen = true;
            gameObject.SetActive(true);
            ModuleSlot = slot;
            Relocate((Vector2)Input.mousePosition - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + offset);
            moduleMenuLoader.UpdateCategory(PlayerController.Instance.Actor.availableModulePrefabs.ToArray(), false);
        }
        
        public void Close()
        {
            IsOpen = false;
            ModuleSlot = null;
            moduleMenuLoader.hoverText.Deactivate();
            gameObject.SetActive(false);
        }

        private void Relocate(Vector2 pos)
        {
            movableObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}