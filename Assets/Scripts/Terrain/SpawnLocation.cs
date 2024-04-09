using System;
using System.Linq;
using Core;
using Entity.Unit;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Terrain
{
    public class SpawnLocation : MonoBehaviour
    {
        [field: SerializeField]
        public Entity.Entity Entity { get; private set; }
        
        public Side Side => side;

        [SerializeField]
        private Side side;

        public Actor Actor { get; set; }

        public Node Node => node;

        [SerializeField]
        private Node node;

        [SerializeField]
        private GameObject drillArrow;

        private SpriteRenderer _arrowRenderer;

        [SerializeField]
        private float drillCost = 10;

        private MeshRenderer[] _renderers;

        private bool _hidden;

        //Returns whether the spawn was successful.
        public GameObject SpawnEntity(GameObject prefab)
        {
            if (Entity) return null;
            var spawnedGameObject = Instantiate(prefab, transform);
            if (!spawnedGameObject.TryGetComponent(out Entity.Entity entityComp))
            {
                Destroy(spawnedGameObject);
                return null;
            }

            Entity = entityComp;
            Entity.Actor = Actor;
            Actor.Entities.Add(Entity);
            Entity.SpawnLocation = this;
            return spawnedGameObject;
        }

        //Returns whether the set was successful.
        public bool SetEntity(Entity.Entity entity)
        {
            if (Entity) return false;
            Entity = entity;
            var entityTransform = entity.transform;
            entityTransform.parent = transform;
            entity.transform.SetLocalPositionAndRotation(Vector3.zero, entity.Actor == Actor ? Quaternion.identity : Quaternion.LookRotation(Vector3.back, Vector3.up));
            Entity.SpawnLocation = this;
            return true;
        }

        public void DetachEntity()
        {
            if (!Entity) return;
            Entity.SpawnLocation = null;
            Entity.transform.parent = null;
            Entity = null;
        }
        
        public void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!Entity)
                {
                    PlayerController.Instance.MouseDown(this);
                }
                else
                {
                    PlayerController.Instance.MouseDown(Entity);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!Entity)
                {
                    PlayerController.Instance.MouseUp(this);
                }
                else
                {
                    PlayerController.Instance.MouseUp(Entity);
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (!Entity)
                {
                    PlayerController.Instance.RightMouseDown(this);
                }
                else
                {
                    PlayerController.Instance.RightMouseDown(Entity);
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (!Entity)
                {
                    PlayerController.Instance.RightMouseUp(this);
                }
                else
                {
                    PlayerController.Instance.RightMouseUp(Entity);
                }
            }
            
            if (!Entity)
            {
                PlayerController.Instance.Hover(this);
            }
            else
            {
                PlayerController.Instance.Hover(Entity);
            }
        }

        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }

        private void Start()
        {
            var baseVisualObjects = GetComponentsInChildren<Transform>().Select(tf => tf.gameObject).Where(go => go.name == "Base").ToArray();
            if (baseVisualObjects.Any())
            {
                Material mat;
                if (side == Side.Home)
                {
                    mat = MatchManager.Instance.HomeColor == TeamColor.Blue
                        ? MatchManager.Instance.BlueMaterial
                        : MatchManager.Instance.OrangeMaterial;
                }
                else
                {
                    mat = MatchManager.Instance.AwayColor == TeamColor.Blue
                        ? MatchManager.Instance.BlueMaterial
                        : MatchManager.Instance.OrangeMaterial;
                }
                baseVisualObjects[0].GetComponent<MeshRenderer>().material = mat;
            }
            
            if (!drillArrow) return;
            _arrowRenderer = drillArrow.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (MatchManager.Instance.MatchState == MatchState.Simulation)
            {
                if (!_hidden)
                {
                    HideVisual();
                    _hidden = true;
                }
            }
            else
            {
                if (_hidden)
                {
                    ShowVisual();
                    _hidden = false;
                }
            }
            
            if (!drillArrow) return;
            _arrowRenderer.enabled = false;
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            if (!node) return;
            if (Entity) return;
            if (PlayerController.Instance.Actor.Currency < drillCost) return;
            _arrowRenderer.enabled = true;
        }

        private void HideVisual()
        {
            foreach (var rdr in _renderers)
            {
                rdr.enabled = false;
            }
        }

        private void ShowVisual()
        {
            foreach (var rdr in _renderers)
            {
                rdr.enabled = true;
            }
        }
    }
}