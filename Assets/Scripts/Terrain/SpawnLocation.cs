using System;
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
            entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Entity.Actor = Actor;
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
        
        private void OnMouseOver()
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
        }

        private void Start()
        {
            if (!drillArrow) return;
            _arrowRenderer = drillArrow.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!drillArrow) return;
            _arrowRenderer.enabled = false;
            if (MatchManager.Instance.MatchState == MatchState.Simulation) return;
            if (!node) return;
            if (Entity) return;
            if (PlayerController.Instance.Actor.Currency < drillCost) return;
            _arrowRenderer.enabled = true;
        }
    }
}