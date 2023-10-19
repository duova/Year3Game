using System;
using System.Linq;
using Entity.Module;
using Entity.Unit;
using Terrain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class PlayerController : Controller
    {
        public static PlayerController Instance { get; private set; }

        public TMP_Text currency;

        public TMP_Text timer;
        
        public TMP_Text phase;

        public GameObject Selected { get; set; }

        public GameObject SelectedImage { get; set; }

        private bool _queueNextFrameSelectReset;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            Instance = this;
        }

        private void FixedUpdate()
        {
            currency.text = "$" + Actor.currency;
            timer.text = "Time remaining in phase: " + MatchManager.Instance.RemainingStateTime;
            phase.text = "Current phase: " + (MatchManager.Instance.MatchState == MatchState.Strategy
                ? "Planning"
                : "Combat");
        }

        public void SelectObjectWithImage(GameObject go, Sprite image)
        {
            Selected = go;
            SelectedImage = new GameObject();
            SelectedImage.transform.eulerAngles = new Vector3(90, 0, 0);
            SelectedImage.AddComponent<Image>().sprite = image;
        }

        public void SelectUnit(GameObject unit)
        {
            Selected = unit;
            if (SelectedImage)
            {
                Destroy(SelectedImage);
                SelectedImage = null;
            }
        }

        public void Ready()
        {
            Actor.Ready();
        }

        private void Update()
        {
            if (SelectedImage && _camera)
            {
                SelectedImage.transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
            
            if (_queueNextFrameSelectReset)
            {
                Selected = null;
                if (SelectedImage)
                {
                    Destroy(SelectedImage);
                    SelectedImage = null;
                }

                _queueNextFrameSelectReset = false;
            }
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _queueNextFrameSelectReset = true;
            }
        }

        public void MouseUp(Entity.Entity entity)
        {
            if (Selected.TryGetComponent(out Unit unit))
            {
                unit.Order = new UnitOrder(OrderType.Follow, entity.gameObject);
            }
            if (Selected.TryGetComponent(out Module module))
            {
                foreach (var slot in entity.ModuleSlots)
                {
                    if (module.GetModuleTypes().Contains(slot.slotType))
                    {
                        slot.InstallModule(module.gameObject);
                        return;
                    }
                }
            }
        }

        public void MouseUp(SpawnLocation location)
        {
            if (Actor.availableEntityPrefabs.Contains(Selected))
            {
                Actor.PurchaseEntity(Selected, location);
            }
            else
            {
                if (Selected.TryGetComponent(out Unit unit))
                {
                    unit.Order = new UnitOrder(OrderType.Move, location.gameObject);
                }
            }
        }
    }
}