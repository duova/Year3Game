﻿using System;
using System.Linq;
using Entity.Module;
using Entity.Unit;
using Terrain;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    public class PlayerController : Controller
    {
        public static PlayerController Instance { get; private set; }

        public TMP_Text currency;

        public TMP_Text timer;
        
        public TMP_Text phase;

        [SerializeField]
        private float flySpeed;

        [field: SerializeField]
        public GameObject Selected { get; set; }

        public GameObject SelectedImage { get; set; }

        private bool _queueNextFrameSelectReset;
        
        private Camera _camera;

        private Vector3 _originalCameraPosition;

        private Quaternion _originalCameraRotation;

        [SerializeField]
        private float sensitivity;

        private float _yRotLimit = 88f;

        [SerializeField]
        private Vector2 originalCameraV2Rotation;
        
        private Vector2 _rotation = Vector2.zero;
        private const string XAxis = "Mouse X";
        private const string YAxis = "Mouse Y";
        
        [SerializeField]
        public MenuLoader spawnMenuLoader;

        [SerializeField]
        private GameObject canvas;

        private LineRenderer _selectedUnitLineRenderer;

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            Instance = this;
            var camTransform = _camera.transform;
            if (camTransform != null)
            {
                _originalCameraPosition = camTransform.position;
                _originalCameraRotation = camTransform.rotation;
                _rotation = originalCameraV2Rotation;
            }
        }

        private void FixedUpdate()
        {
            if (_selectedUnitLineRenderer != null && Selected != null)
            {
                var cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
                var xzPlane = new Plane(Vector3.up, Vector3.zero);
                xzPlane.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out float hitDistance);
                var point = cameraRay.GetPoint(hitDistance);
                _selectedUnitLineRenderer.SetPositions(new []{Selected.transform.position, point});
            }
            
            currency.text = "$" + Actor.Currency;
            if (currency.color != Color.white)
            {
                var color = currency.color;
                color.g = currency.color.g + 0.02f;
                color.b = currency.color.b + 0.02f;
                currency.color = color;
            }
            timer.text = "Time: " + (int)MatchManager.Instance.RemainingStateTime;
            phase.text = "Phase: " + (MatchManager.Instance.MatchState == MatchState.Strategy
                ? "Planning"
                : "Combat");

            if (MatchManager.Instance.MatchState == MatchState.Strategy)
            {
                var camTransform = _camera.transform;
                camTransform.position = _originalCameraPosition;
                camTransform.rotation = _originalCameraRotation;
            }
        }

        public void SelectObjectWithImage(GameObject go, Sprite image)
        {
            Selected = go;
            if (go.TryGetComponent(out Module module))
            {
                if (!Actor.PurchasedModulePrefabs.Contains(go))
                {
                    if (!Actor.PurchaseModule(go))
                    {
                        currency.color = Color.red;
                        DeductCurrencyText.Instance.NoMoney();
                    }
                    return;
                }
                else
                {
                    if (ModuleMenu.Instance.IsOpen)
                    {
                        var slot = ModuleMenu.Instance.ModuleSlot;
                        if (module.GetModuleTypes().Contains(slot.slotType))
                        {
                            if (slot.Module != null)
                            {
                                slot.UninstallModule();
                            }
                            slot.InstallModule(module.gameObject);
                        }
                        EntityMenu.Instance.PresentSlots();
                        ModuleMenu.Instance.Close();
                    }
                }
            }

            if (go.TryGetComponent(out Entity.Entity entity))
            {
                if (Actor.PurchaseEntity(Selected, SpawnMenu.Instance.SpawnLocation))
                {
                    SpawnMenu.Instance.Close();
                }
                else
                {
                    currency.color = Color.red;
                    DeductCurrencyText.Instance.NoMoney();
                }
            }

            Selected = null;
        }

        public void Ready()
        {
            Actor.Ready();
        }

        private void Update()
        {
            var camTransform = _camera.transform;
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                ClearMouseState();
                if (SpawnMenu.Instance.IsOpen)
                {
                    SpawnMenu.Instance.Close();
                }
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                var right = camTransform.right;
                var forward = -Vector3.Cross(right, Vector3.down);
                var up = Vector3.up;
                MoveInDirection(KeyCode.W, forward);
                MoveInDirection(KeyCode.A, -right);
                MoveInDirection(KeyCode.S, -forward);
                MoveInDirection(KeyCode.D, right);
                MoveInDirection(KeyCode.Space, up);
                MoveInDirection(KeyCode.LeftControl, -up);
                
                _rotation.x += Input.GetAxis(XAxis) * sensitivity;
                _rotation.y += Input.GetAxis(YAxis) * sensitivity;
                _rotation.y = Mathf.Clamp(_rotation.y, -_yRotLimit, _yRotLimit);
                var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
                var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

                camTransform.rotation = xQuat * yQuat;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                camTransform.rotation = _originalCameraRotation;
                camTransform.position = _originalCameraPosition;
                _rotation = originalCameraV2Rotation;
            }
            
            if (SelectedImage && _camera)
            {
                SelectedImage.transform.position = Input.mousePosition;
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
                if (_selectedUnitLineRenderer)
                {
                    _selectedUnitLineRenderer.positionCount = 0;
                    _selectedUnitLineRenderer = null;
                }
                _queueNextFrameSelectReset = true;
            }
        }

        public void MouseUp(Entity.Entity entity)
        {
            if (EntityMenu.Instance.IsOpen || SpawnMenu.Instance.IsOpen) return;
            
            if (EntityMenu.Instance.Entity == null && entity.Actor == Actor)
            {
                EntityMenu.Instance.Open(entity);
            }
        }

        public void MouseUp(SpawnLocation location)
        {
            if (EntityMenu.Instance.IsOpen || SpawnMenu.Instance.IsOpen) return;
            
            if (SpawnMenu.Instance.SpawnLocation == null && location.Actor == Actor)
            {
                SpawnMenu.Instance.Open(location);
            }
        }

        public void MouseDown(Entity.Entity entity)
        {
        }
        
        public void MouseDown(SpawnLocation location)
        {
        }
        
        public void RightMouseUp(Entity.Entity entity)
        {
            if (!Selected) return;
            if (Selected.TryGetComponent(out Unit unit) && !Actor.availableEntityPrefabs.Contains(Selected))
            {
                if (Selected == entity.gameObject)
                {
                    //If clicking on itself it should go back to stay.
                    Actor.GiveOrder(OrderType.Stay, null, unit, true);
                }
                else
                {
                    Actor.GiveOrder(OrderType.Follow, entity.gameObject, unit, true);
                }
            }
            if (Selected.TryGetComponent(out Module module))
            {
                foreach (var slot in entity.ModuleSlots)
                {
                    if (module.GetModuleTypes().Contains(slot.slotType))
                    {
                        if (slot.Module != null) continue;
                        slot.InstallModule(module.gameObject);
                        return;
                    }
                }
            }
            
            Selected = null;
            _selectedUnitLineRenderer = null;
        }

        public void RightMouseUp(SpawnLocation location)
        {
            if (Selected && Selected.TryGetComponent(out Unit unit))
            {
                Actor.GiveOrder(OrderType.Move, location.gameObject, unit, true);
            }
            Selected = null;
            _selectedUnitLineRenderer = null;
        }

        public void RightMouseDown(Entity.Entity entity)
        {
            if (Actor.availableEntityPrefabs.Contains(entity.gameObject)) return;
            if (entity.TryGetComponent<Unit>(out _))
            {
                Selected = entity.gameObject;
                _selectedUnitLineRenderer = entity.gameObject.GetComponent<LineRenderer>();
                _selectedUnitLineRenderer.positionCount = 2;
            }
        }
        
        public void RightMouseDown(SpawnLocation location)
        {
        }

        private void MoveInDirection(KeyCode key, Vector3 direction)
        {
            if (Input.GetKey(key))
            {
                _camera.transform.position += direction * (flySpeed * Time.deltaTime);
            }
        }

        private void ClearMouseState()
        {
            if (_selectedUnitLineRenderer != null)
            {
                _selectedUnitLineRenderer.positionCount = 0;
            }
            _selectedUnitLineRenderer = null;
            Selected = null;
        }
    }
}