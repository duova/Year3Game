using System;
using UnityEditor;
using UnityEngine;

namespace Terrain
{
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(TileSpawner))]
    public class TileSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("NE"))
            {
                ((TileSpawner)target).SpawnNe();
            }
            if (GUILayout.Button("NW"))
            {
                ((TileSpawner)target).SpawnNw();
            }
            if (GUILayout.Button("E"))
            {
                ((TileSpawner)target).SpawnE();
            }
            if (GUILayout.Button("W"))
            {
                ((TileSpawner)target).SpawnW();
            }
            if (GUILayout.Button("SE"))
            {
                ((TileSpawner)target).SpawnSe();
            }
            if (GUILayout.Button("SW"))
            {
                ((TileSpawner)target).SpawnSw();
            }
        }
    }
    
    #endif
    
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private float hexLongestWidth;

        public void SpawnNe()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(shortHorizontalOffset, verticalOffset);
        }
        
        public void SpawnNw()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(-shortHorizontalOffset, verticalOffset);
        }
        
        public void SpawnE()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(longHorizontalOffset, 0);
        }
        
        public void SpawnW()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(-longHorizontalOffset, 0);
        }
        
        public void SpawnSe()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(shortHorizontalOffset, -verticalOffset);
        }
        
        public void SpawnSw()
        {
            var halfWidth = hexLongestWidth / 2;
            var shortHorizontalOffset = Mathf.Cos((float)Math.PI / 6f) * halfWidth;
            var longHorizontalOffset = shortHorizontalOffset * 2f;
            var verticalOffset = 1.5f * halfWidth;
            
            InternalSpawn(-shortHorizontalOffset, -verticalOffset);
        }

        private void InternalSpawn(float horizontalOffset, float verticalOffset)
        {
            Instantiate(prefab, transform.position + new Vector3(horizontalOffset, 0, verticalOffset),
                Quaternion.identity);
        }
    }
}