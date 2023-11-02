using System;
using System.Linq;
using Entity.Module;
using Entity.Unit;
using Terrain;
using TMPro;
using UI;
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
        private ScrollCategory buildCategory;
        
        [SerializeField]
        private ScrollCategory unitCategory;
        
        [SerializeField]
        private ScrollCategory modCategory;

        [SerializeField]
        private GameObject canvas;

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

        protected void Start()
        {
            buildCategory.UpdateCategory(Actor.availableEntityPrefabs.Where(entity => !entity.TryGetComponent<Unit>(out _)).ToArray());
            unitCategory.UpdateCategory(Actor.availableEntityPrefabs.Where(entity => entity.TryGetComponent<Unit>(out _)).ToArray());
            modCategory.UpdateCategory(Actor.availableModulePrefabs);
        }

        private void FixedUpdate()
        {
            currency.text = "$" + Actor.currency;
            timer.text = "Time remaining in phase: " + (int)MatchManager.Instance.RemainingStateTime;
            phase.text = "Current phase: " + (MatchManager.Instance.MatchState == MatchState.Strategy
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
                    Actor.PurchaseModule(go);
                    modCategory.UpdateCategory(Actor.availableModulePrefabs);
                    return;
                }
            }
            SelectedImage = new GameObject();
            SelectedImage.transform.parent = canvas.transform;
            var imageComp = SelectedImage.AddComponent<Image>();
            imageComp.sprite = image;
            imageComp.color = new Color(imageComp.color.r, imageComp.color.g, imageComp.color.b, 0.5f);
            imageComp.raycastTarget = false;
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
                _queueNextFrameSelectReset = true;
            }
        }

        public void MouseUp(Entity.Entity entity)
        {
            if (Selected.TryGetComponent(out Unit unit) && !Actor.availableEntityPrefabs.Contains(Selected))
            {
                Actor.GiveOrder(OrderType.Follow, entity.gameObject, unit);
            }
            if (Selected.TryGetComponent(out Module module))
            {
                foreach (var slot in entity.ModuleSlots)
                {
                    if (module.GetModuleTypes().Contains(slot.slotType))
                    {
                        slot.UninstallModule();
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
                if (Selected && Selected.TryGetComponent(out Unit unit))
                {
                    Actor.GiveOrder(OrderType.Move, location.gameObject, unit);
                }
            }
        }

        public void MouseDown(Entity.Entity entity)
        {
            if (Actor.availableEntityPrefabs.Contains(entity.gameObject)) return;
            if (entity.TryGetComponent<Unit>(out _))
            {
                Selected = entity.gameObject;
            }
        }

        private void MoveInDirection(KeyCode key, Vector3 direction)
        {
            if (Input.GetKey(key))
            {
                _camera.transform.position += direction * (flySpeed * Time.deltaTime);
            }
        }
    }
}