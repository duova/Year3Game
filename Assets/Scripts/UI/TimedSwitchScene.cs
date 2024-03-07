using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TimedSwitchScene : MonoBehaviour
    {
        [SerializeField]
        private int scene;
        
        [SerializeField]
        private float seconds;

        private float _time = 0;

        private void Update()
        {
            _time += Time.deltaTime;
            if (_time >= seconds)
            {
                SceneManager.LoadScene(scene);
            }
        }
    }
}