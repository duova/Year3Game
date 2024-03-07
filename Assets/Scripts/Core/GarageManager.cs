using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Module;
using Entity.Structure;
using Terrain;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Core
{
    public struct EntitySave
    {
        public GameObject EntityPrefab;

        public GameObject[] ModulePrefabs;

        public int Cost;

        public string Name;
        
        public Texture2D Image;
    }
    
    public class GarageManager : MonoBehaviour
    {
        public static GarageManager Instance { get; private set; }

        public EntitySave[] EntitySaves { get; private set; }

        [SerializeField]
        private GameObject cameraMarker;
        
        public SpawnLocation spawnLocation;

        [SerializeField]
        private MenuLoader entityMenu;
        
        [SerializeField]
        private MenuLoader moduleMenu;

        [SerializeField]
        private GameObject slotMenu;

        [SerializeField]
        private int numEntitySaves;

        [SerializeField]
        private GameObject[] moduleSlotButtons;

        [SerializeField]
        private Sprite defaultSlotImage;

        [SerializeField]
        private GameObject mainCanvas;

        [SerializeField]
        private GameObject garageCanvas;
        
        [SerializeField]
        private TMP_InputField nameField;
        
        [SerializeField]
        private GameObject buttonPrefab;

        public bool inGarage;

        public int CurrentSaveSlotIndex { get; private set; }

        public int SelectedModuleIndex { get; private set; }

        private GameObject _currentEntityPrefab;

        private GameObject[] _currentModulePrefabs;

        private bool _savingScreenshot;

        public List<AddedEventsButton> SlotButtons { get; private set; } = new ();

        private void Awake()
        {
            Instance = this;
            EntitySaves = new EntitySave[numEntitySaves];
            
            _camera = Camera.main;
    
            if(!_renderTexture)
            {
                _renderTexture = new RenderTexture(Screen.height, Screen.height, 24 , RenderTextureFormat.ARGB32);
                _renderTexture.useMipMap = false;
                _renderTexture.antiAliasing =1;
            }

            foreach (var button in moduleSlotButtons)
            {
                button.SetActive(false);
            }
        }

        private void Start()
        {
            garageCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            UpdateSlotMenu();
        }

        public void SetSaveSlotIndex(int saveIndex)
        {
            CurrentSaveSlotIndex = saveIndex;
        }

        public void GoToGarage()
        {
            if (MatchManager.Instance.MatchState != MatchState.Strategy) return;
            mainCanvas.SetActive(false);
            garageCanvas.SetActive(true);
            inGarage = true;
            SelectedModuleIndex = 0;
            var transform1 = Camera.main.transform;
            transform1.position = cameraMarker.transform.position;
            transform1.rotation = cameraMarker.transform.rotation;

            if (EntitySaves[CurrentSaveSlotIndex].EntityPrefab)
            {
                //Load slot.
                SelectEntity(EntitySaves[CurrentSaveSlotIndex].EntityPrefab);
                for (var i = 0; i < EntitySaves[CurrentSaveSlotIndex].ModulePrefabs.Length; i++)
                {
                    var prefab = EntitySaves[CurrentSaveSlotIndex].ModulePrefabs[i];
                    SelectModuleSlot(i);
                    SelectModule(prefab);
                }

                nameField.text = EntitySaves[CurrentSaveSlotIndex].Name;
            }

            UpdateGrids();
        }

        public void SelectEntity(GameObject prefab)
        {
            if (!prefab) return;
            
            if (spawnLocation.Entity != null)
            {
                var entityToDestroy = spawnLocation.Entity;
                entityToDestroy.Detach();
                Destroy(entityToDestroy.gameObject);
                _currentModulePrefabs = null;
            }

            spawnLocation.Actor.PurchaseEntity(prefab, spawnLocation, false);
            
            foreach (var button in moduleSlotButtons)
            {
                button.SetActive(false);
            }
            
            for (var i = 0; i < spawnLocation.Entity.ModuleSlots.Length; i++)
            {
                moduleSlotButtons[i].SetActive(true);
            }

            _currentEntityPrefab = prefab;
            _currentModulePrefabs = new GameObject[spawnLocation.Entity.ModuleSlots.Length];
            SelectModuleSlot(0);
        }

        public void SelectModule(GameObject prefab)
        {
            if (!prefab) return;
            if (_currentEntityPrefab == null) return;
            if (spawnLocation.Entity.ModuleSlots.Length <= SelectedModuleIndex) throw new Exception("Garage selected module index is out of bounds");
            var slot = spawnLocation.Entity.ModuleSlots[SelectedModuleIndex];
            if (slot.Module != null)
            {
                slot.UninstallModule();
            }
            slot.InstallModule(prefab);
            _currentModulePrefabs[SelectedModuleIndex] = prefab;
        }

        //Auto save.
        public void LeaveGarage()
        {
            if (_savingScreenshot) return;
            
            if (_currentEntityPrefab)
            {
                //Save data.
                EntitySaves[CurrentSaveSlotIndex].Name = nameField.text;
                EntitySaves[CurrentSaveSlotIndex].EntityPrefab = _currentEntityPrefab;
                EntitySaves[CurrentSaveSlotIndex].ModulePrefabs = _currentModulePrefabs;
                EntitySaves[CurrentSaveSlotIndex].Cost = 0;
                EntitySaves[CurrentSaveSlotIndex].Cost += _currentEntityPrefab.GetComponent<Entity.Entity>().price;
                foreach (var module in _currentModulePrefabs)
                {
                    if (!module) continue;
                    EntitySaves[CurrentSaveSlotIndex].Cost += module.GetComponent<Module>().price;
                }

                //Save screenshot.
                _savingScreenshot = true;
                Snapshot(UpdateSlotImage, CurrentSaveSlotIndex);
            }

            //Reset.
            var entity = spawnLocation.Entity;
            if (entity)
            {
                entity.Detach();
                entity.Destroy();
            }
            _currentEntityPrefab = null;
            _currentModulePrefabs = null;
            inGarage = false;
            mainCanvas.SetActive(true);
            garageCanvas.SetActive(false);
        }

        public void SelectModuleSlot(int index)
        {
            SelectedModuleIndex = index;
            foreach (var go in moduleSlotButtons)
            {
                if (go.transform.childCount > 0)
                {
                    go.transform.GetChild(0).gameObject.SetActive(false);
                }

                go.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }

            if (moduleSlotButtons.Length > index)
            {
                var tf = moduleSlotButtons[index].transform;
                if (tf.childCount > 0)
                {
                    tf.GetChild(0).gameObject.SetActive(true);
                }
                moduleSlotButtons[index].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        
        private void UpdateGrids()
        {
            entityMenu.UpdateCategory(PlayerController.Instance.Actor.availableEntityPrefabs.Where(prefab => !prefab.TryGetComponent<Drill>(out _)).ToArray());
            moduleMenu.UpdateCategory(PlayerController.Instance.Actor.availableModulePrefabs);
        }

        private void UpdateSlotImage(int index, Texture2D image)
        {
            EntitySaves[index].Image = image;
            UpdateSlotMenu();
        }
        
        private Camera _camera;
        
        private RenderTexture _renderTexture;
    
        public void Snapshot(Action<int, Texture2D> onSnapshotDone, int saveIndex)
        {
            SnapshotRoutine(onSnapshotDone, saveIndex);
        }
    
        private void SnapshotRoutine (Action<int, Texture2D> onSnapshotDone, int saveIndex)
        {
            // If RenderTexture.active is set any rendering goes into this RenderTexture
            // instead of the GameView
            RenderTexture.active = _renderTexture;
            _camera.targetTexture = _renderTexture;
               
            // renders into the renderTexture
            _camera.Render();
     
            // Create a new Texture2D        
            var result = new Texture2D(Screen.height,Screen.height,TextureFormat.ARGB32,false);
            // copies the pixels into the Texture2D          
            result.ReadPixels(new Rect(0,0,Screen.height,Screen.height),0,0,false);
            result.Apply();
       
            // reset the RenderTexture.active so nothing else is rendered into our RenderTexture      
            RenderTexture.active = null;
            _camera.targetTexture = null;
    
            // Invoke the callback with the resulting snapshot Texture
            onSnapshotDone?.Invoke(saveIndex, result);

            _savingScreenshot = false;
        }
        
        public void UpdateSlotMenu()
        {
            foreach (var go in slotMenu.GetComponentsInChildren<Transform>().Select(t => t.gameObject))
            {
                if (go == slotMenu) continue;
                Destroy(go);
            }
            
            SlotButtons.Clear();

            for (var i = 0; i < EntitySaves.Length; i++)
            {
                var save = EntitySaves[i];
                var image = save.Image;
                var desc = save.Name + "\n" + "$" + save.Cost;

                var newGo = Instantiate(buttonPrefab, slotMenu.transform);
                var textComp = newGo.GetComponentInChildren<TMP_Text>();
                var imageComp = newGo.GetComponent<Image>();
                if (image == null)
                {
                    imageComp.sprite = defaultSlotImage;
                }
                else
                {
                    imageComp.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
                }

                textComp.text = desc;

                var buttonComp = newGo.GetComponent<AddedEventsButton>();
                buttonComp.OnUp += OnClicked;
                buttonComp.OnEnter += OnEnter;
                buttonComp.OnExit += OnExit;

                SlotButtons.Add(buttonComp);

                if (CurrentSaveSlotIndex == i)
                {
                    foreach (var child in newGo.GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.SetActive(true);
                    }

                    imageComp.color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
        
        private void OnClicked(AddedEventsButton buttonComp)
        {
            var index = SlotButtons.IndexOf(buttonComp);
            SetSaveSlotIndex(index);
            UpdateSlotMenu();
        }
        
        private void OnEnter(AddedEventsButton buttonComp)
        {
            var index = SlotButtons.IndexOf(buttonComp);
        }
        
        private void OnExit(AddedEventsButton buttonComp)
        {
            var index = SlotButtons.IndexOf(buttonComp);
        }
    }
}