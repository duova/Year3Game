using UnityEngine;

namespace Terrain
{
    public class SpawnLocation : MonoBehaviour
    {
        public GameObject Entity { get; private set; }

        [SerializeField]
        private GameObject _node;
    }
}