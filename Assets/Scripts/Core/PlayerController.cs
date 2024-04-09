using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Module;
using Entity.Structure;
using Entity.Unit;
using Terrain;
using TMPro;
using UI;
using UnityEngine;

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
        private GameObject cameraMaxPos;
        
        [SerializeField]
        private GameObject cameraMinPos;

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

        [SerializeField]
        private float cameraRotationRate;
        
        [SerializeField]
        private float cameraVerticalSpeed;

        private List<Unit> _selectedUnits = new();

        private Entity.Entity _hoveredEntity;
        
        private float roundDurationCounter = 0;

        [SerializeField]
        private float roundDisplayDuration = 1f;

        private float roundDisplayDurationTimer;

        [SerializeField]
        private AnimatedBox roundDisplayBox;
        
        [SerializeField]
        private TMP_Text roundDisplayText;

        public float gainedCurrencyThisRound;

        public float upkeepThisRound;

        [SerializeField]
        private HoverText moduleOneText;
        
        [SerializeField]
        private HoverText moduleTwoText;
        
        [SerializeField]
        private HoverText moduleThreeText;

        [SerializeField]
        private float modInfoHoldTime;

        private float _modInfoHoldTimer;

        private Entity.Entity _modInfoHoldEntity;

        private bool _wasdClicked;

        private bool _scrollUsed;

        private int _numPurchases;

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
            
            currency.text = "$" + Actor.Currency + "/" + Actor.maxCurrency;
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
        }

        public void SelectObjectWithImage(GameObject go, Sprite image, bool garage = false)
        {
            if (garage)
            {
                if (go.TryGetComponent(out Module module))
                {
                    GarageManager.Instance.SelectModule(go);
                }

                if (go.TryGetComponent(out Entity.Entity entity))
                {
                    GarageManager.Instance.SelectEntity(go);
                }
            }
            else
            {
                Selected = go;
                /*
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
                */
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
        }

        public void Ready()
        {
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.ConditionalGoToSection(6, 7);
                TutorialManager.Instance.ConditionalGoToSection(18, 19);
                TutorialManager.Instance.ConditionalGoToSection(34, 35);
            }
            Actor.Ready();
        }

        private void Update()
        {
            if (TutorialManager.Instance && TutorialManager.Instance.GetSection() > 0)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
                    Input.GetKey(KeyCode.D))
                {
                    _wasdClicked = true;
                }
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                _scrollUsed = true;
            }
            
            if (_wasdClicked && _scrollUsed)
            {
                if (TutorialManager.Instance)
                {
                    TutorialManager.Instance.ConditionalGoToSection(1, 2);
                }
            }
            
            roundDisplayDurationTimer += Time.deltaTime;
            if (roundDisplayDurationTimer > roundDisplayDuration)
            {
                roundDisplayBox.Close();
            }

            if (_modInfoHoldTimer > 0.01f)
            {
                _modInfoHoldTimer += Time.deltaTime;
            }

            if (_modInfoHoldTimer > modInfoHoldTime)
            {
                if (GarageManager.Instance.inGarage) return;
                ModuleSlot slot = null;
                switch (_modInfoHoldEntity.ModuleSlots.Length)
                {
                    case 3:
                        slot = _modInfoHoldEntity.ModuleSlots[2];
                        if (slot.Module)
                        {
                            moduleThreeText.Activate(slot.Module.text);
                        }
                        goto case 2;
                    case 2:
                        slot = _modInfoHoldEntity.ModuleSlots[1];
                        if (slot.Module)
                        {
                            moduleTwoText.Activate(slot.Module.text);
                        }
                        goto case 1;
                    case 1:
                        slot = _modInfoHoldEntity.ModuleSlots[0];
                        if (slot.Module)
                        {
                            moduleOneText.Activate(slot.Module.text);
                        }
                        break;
                }

                _modInfoHoldTimer = 0f;
            }
            
            var camTransform = _camera.transform;
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                roundDurationCounter += Time.deltaTime;
                ClearMouseState();
                if (SpawnMenu.Instance.IsOpen)
                {
                    SpawnMenu.Instance.Close();
                }
                
                /*
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
                */
            }
            
            if (GarageManager.Instance && !GarageManager.Instance.inGarage)
            {
                var right = camTransform.right;
                var forward = -Vector3.Cross(right, Vector3.down);
                var up = Vector3.up;
                if (camTransform.position.z < cameraMaxPos.transform.position.z)
                {
                    MoveInDirection(KeyCode.W, forward);
                }

                if (camTransform.position.x > cameraMinPos.transform.position.x)
                {
                    MoveInDirection(KeyCode.A, -right);
                }

                if (camTransform.position.z > cameraMinPos.transform.position.z)
                {
                    MoveInDirection(KeyCode.S, -forward);
                }

                if (camTransform.position.x < cameraMaxPos.transform.position.x)
                {
                    MoveInDirection(KeyCode.D, right);
                }

                if (Input.mouseScrollDelta.y < 0 && camTransform.position.y < cameraMaxPos.transform.position.y)
                {
                    _camera.transform.position += new Vector3(0, cameraVerticalSpeed, 0);
                    _camera.transform.rotation = Quaternion.Euler(_camera.transform.rotation.eulerAngles + new Vector3(cameraRotationRate, 0, 0));
                }
                
                if (Input.mouseScrollDelta.y > 0 && camTransform.position.y > cameraMinPos.transform.position.y)
                {
                    _camera.transform.position -= new Vector3(0, cameraVerticalSpeed, 0);
                    _camera.transform.rotation = Quaternion.Euler(_camera.transform.rotation.eulerAngles + new Vector3(-cameraRotationRate, 0, 0));
                }
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
            
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (_selectedUnitLineRenderer)
                {
                    _selectedUnitLineRenderer.positionCount = 0;
                    _selectedUnitLineRenderer = null;
                }
                _queueNextFrameSelectReset = true;
            }

            if (!GarageManager.Instance.inGarage)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        ClearSelectedUnits();
                    }

                    if (_hoveredEntity)
                    {
                        if (_hoveredEntity.GetType() == typeof(Unit) && _hoveredEntity.Actor == Actor)
                        {
                            if (!_selectedUnits.Contains(_hoveredEntity))
                            {
                                _selectedUnits.Add((Unit)_hoveredEntity);
                                _hoveredEntity.SpawnLocation.GetComponent<TileHighlight>().Override = true;
                            }
                        }
                    }
                    
                    if (_selectedUnits.Count > 1)
                    {
                        if (TutorialManager.Instance)
                        {
                            TutorialManager.Instance.ConditionalGoToSection(2, 3);
                        }
                    }
                }
            }

            _hoveredEntity = null;
        }

        public void ClearSelectedUnits()
        {
            foreach (var unit in _selectedUnits)
            {
                unit.SpawnLocation.GetComponent<TileHighlight>().Override = false;
            }

            _selectedUnits.Clear();
        }

        public void MouseUp(Entity.Entity entity)
        {
            moduleThreeText.Deactivate();
            moduleTwoText.Deactivate();
            moduleOneText.Deactivate();
            _modInfoHoldTimer = 0;
            _modInfoHoldEntity = null;
            
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            if (EntityMenu.Instance.IsOpen || SpawnMenu.Instance.IsOpen) return;
            
            /*
            if (EntityMenu.Instance.Entity == null && entity.Actor == Actor)
            {
                EntityMenu.Instance.Open(entity);
            }
            */
        }

        public void MouseUp(SpawnLocation location)
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            if (EntityMenu.Instance.IsOpen || SpawnMenu.Instance.IsOpen) return;

            if (location.Actor != Actor) return;

            if (location.Node)
            {
                if (SpawnMenu.Instance.SpawnLocation == null && location.Actor == Actor)
                {
                    SpawnMenu.Instance.Open(location);
                }
            }
            else
            {
                var save = GarageManager.Instance.EntitySaves[GarageManager.Instance.CurrentSaveSlotIndex];

                if (save.EntityPrefab == null) return;

                if (Actor.Currency < save.Cost)
                {
                    DeductCurrencyText.Instance.NoMoney();
                    return;
                }

                Actor.Currency -= save.Cost;

                Actor.PurchaseEntity(save.EntityPrefab, location, false);

                for (var i = 0; i < save.ModulePrefabs.Length; i++)
                {
                    var module = save.ModulePrefabs[i];
                    if (!module) continue;
                    Actor.InstallModule(module, location.Entity.ModuleSlots[i], false);
                }

                _numPurchases++;
                
                if (TutorialManager.Instance && _numPurchases > 1)
                {
                    TutorialManager.Instance.ConditionalGoToSection(0, 1);
                }

                if (TutorialManager.Instance && Actor.SpawnLocations.All(location => location.Entity != null || location == GarageManager.Instance.spawnLocation))
                {
                    TutorialManager.Instance.ConditionalGoToSection(32, 33);
                }
            }
        }

        public void MouseDown(Entity.Entity entity)
        {
            if (GarageManager.Instance.inGarage) return;
            _modInfoHoldTimer = 0.02f;
            _modInfoHoldEntity = entity;
        }
        
        public void MouseDown(SpawnLocation location)
        {
        }
        
        public void RightMouseUp(Entity.Entity entity)
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;

            if (TutorialManager.Instance && (TutorialManager.Instance.GetSection() == 6 || TutorialManager.Instance.GetSection() == 18 || TutorialManager.Instance.GetSection() == 34)) return;
            
            foreach (var unit in _selectedUnits)
            {
                {
                    Actor.GiveOrder(OrderType.Follow, entity.gameObject, unit, true);
                }
            }

            if (entity.Actor == Actor)
            {
                if (TutorialManager.Instance)
                {
                    TutorialManager.Instance.ConditionalGoToSection(4, 5);
                }
                if (entity.GetType() == typeof(Objective) && TutorialManager.Instance && Actor.Entities.Count(itEntity => itEntity.GetType() == typeof(Unit)) > 1)
                {
                    if (Actor.Entities.Where(itEntity => itEntity.GetType() == typeof(Unit))
                        .All(unitEntity => ((Unit)unitEntity).Order.Target == entity.gameObject))
                    {
                        TutorialManager.Instance.ConditionalGoToSection(17, 18);
                    }
                }
            }
            else
            {
                if (entity.GetType() == typeof(Objective) && TutorialManager.Instance)
                {
                    if (Actor.Entities.Where(itEntity => itEntity.GetType() == typeof(Unit))
                        .All(unitEntity => ((Unit)unitEntity).Order.Target == entity.gameObject))
                    {
                        TutorialManager.Instance.ConditionalGoToSection(5, 6);
                        TutorialManager.Instance.ConditionalGoToSection(33, 34);
                    }
                }
            }
            
            /*
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
            */
            /*
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
            */
            
            Selected = null;
            _selectedUnitLineRenderer = null;
        }

        public void RightMouseUp(SpawnLocation location)
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            
            if (TutorialManager.Instance && (TutorialManager.Instance.GetSection() == 6 || TutorialManager.Instance.GetSection() == 18 || TutorialManager.Instance.GetSection() == 34)) return;
            
            foreach (var unit in _selectedUnits)
            {
                Actor.GiveOrder(OrderType.Move, location.gameObject, unit, true);
            }
            
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.ConditionalGoToSection(3, 4);
            }
            
            /*
            if (Selected && Selected.TryGetComponent(out Unit unit))
            {
                Actor.GiveOrder(OrderType.Move, location.gameObject, unit, true);
            }
            */
            Selected = null;
            _selectedUnitLineRenderer = null;
        }

        public void RightMouseDown(Entity.Entity entity)
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            /*
            if (Actor.availableEntityPrefabs.Contains(entity.gameObject)) return;
            if (entity.TryGetComponent<Unit>(out _) && entity.Actor == Actor)
            {
                Selected = entity.gameObject;
                _selectedUnitLineRenderer = entity.gameObject.GetComponent<LineRenderer>();
                _selectedUnitLineRenderer.positionCount = 2;
            }
            */
        }
        
        public void RightMouseDown(SpawnLocation location)
        {
        }

        private void MoveInDirection(KeyCode key, Vector3 direction)
        {
            if (Input.GetKey(key))
            {
                _camera.transform.position += direction * (flySpeed * Time.unscaledDeltaTime);
            }
        }
        
        public void Hover(Entity.Entity entity)
        {
            _hoveredEntity = entity;
        }
        
        public void Hover(SpawnLocation spawnLocation)
        {
            
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

        public void ResetCamera()
        {
            var camTransform = _camera.transform;
            camTransform.rotation = _originalCameraRotation;
            camTransform.position = _originalCameraPosition;
            camTransform.rotation = _originalCameraRotation;
        }

        public void OnSimulationEnd()
        {
            PostRoundDisplay();
            roundDurationCounter = 0;
        }

        public void OnSimulationStart()
        {
            roundDisplayDurationTimer = roundDisplayDuration;
        }
        
        private void PostRoundDisplay()
        {
            var displayText = "";
            if (roundDurationCounter < 0.1f)
            {
                displayText += "(neither side gave orders)\n";
            }
            displayText += "Gained $" + gainedCurrencyThisRound + " from drills.\n";
            displayText += "Used $" + upkeepThisRound + " to maintain units ($2 per unit).\n";

            roundDisplayText.text = displayText;

            gainedCurrencyThisRound = 0;
            
            roundDisplayBox.Open();
            roundDisplayDurationTimer = 0;

            upkeepThisRound = 0;
        }
        
        public void MouseEnter(Entity.Entity entity)
        {
        }
    
        public void MouseExit(Entity.Entity entity)
        {
            moduleThreeText.Deactivate();
            moduleTwoText.Deactivate();
            moduleOneText.Deactivate();
            _modInfoHoldTimer = 0;
            _modInfoHoldEntity = null;
        }
    }
}