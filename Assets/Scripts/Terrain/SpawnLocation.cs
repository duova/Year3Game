using Core;
using UnityEditor;
using UnityEngine;

namespace Terrain
{
    public class SpawnLocation : MonoBehaviour
    {
        public Entity.Entity Entity { get; private set; }
        
        public Side Side => side;

        [SerializeField]
        private Side side;

        public Actor Actor { get; set; }

        public Node Node => node;

        [SerializeField]
        private Node node;

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
            entityTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Entity.Actor = Actor;
            Entity.SpawnLocation = this;
            return true;
        }

        public void DetachEntity(Entity.Entity entity)
        {
            if (Entity) return;
            Entity.SpawnLocation = null;
            Entity.transform.parent = null;
            Entity = null;
        }
        
        private void OnMouseUp()
        {
            PlayerController.Instance.MouseUp(this);
        }
    }
}