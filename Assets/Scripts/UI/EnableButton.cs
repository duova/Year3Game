using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class EnableButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] gameObjectsToEnable;
        
        [SerializeField]
        private GameObject[] gameObjectsToDisable;
        
        public void OnPress()
        {
            foreach (var go in gameObjectsToEnable)
            {
                go.SetActive(true);
            }
            foreach (var go in gameObjectsToDisable)
            {
                go.SetActive(false);
            }
        }
    }
}