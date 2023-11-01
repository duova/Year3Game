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

        [SerializeField]
        private float flySpeed;

        public GameObject Selected { get; set; }

        public GameObject SelectedImage { get; set; }

        private bool _queueNextFrameSelectReset;
        
        private Camera _camera;

        private Vector3 _originalCameraPosition;

        private Quaternion _originalCameraRotation;

        [SerializeField]
        private float sensitivity;

        private float _yRotLimit = 88f;
        
        private Vector2 _rotation = Vector2.zero;
        private const string XAxis = "Mouse X";
        private const string YAxis = "Mouse Y";

        private void Awake()
        {
            _camera = Camera.main;
            Instance = this;
            _originalCameraPosition = _camera.transform.position;
            _originalCameraRotation = _camera.transform.rotation;
        }

        private void FixedUpdate()
        {
            currency.text = "$" + Actor.currency;
            timer.text = "Time remaining in phase: " + MatchManager.Instance.RemainingStateTime;
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
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                var camTransform = _camera.transform;
                var forward = camTransform.forward;
                var right = camTransform.right;
                var up = camTransform.up;
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
            }
            
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
                if (Selected.TryGetComponent(out Unit unit))
                {
                    unit.Order = new UnitOrder(OrderType.Move, location.gameObject);
                }
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