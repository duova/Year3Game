using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class AnimatedBox : MonoBehaviour
    {
        private Image _image;

        [SerializeField]
        private float speed = 2f;
        
        [SerializeField]
        private GameObject subObject;

        [SerializeField]
        private bool openOnEnable;

        private float _targetPercentage;

        private float _currentPercentage;
        private static readonly int OpenPercentage = Shader.PropertyToID("_OpenPercentage");

        private bool _active;

        private void Start()
        {
            _image = GetComponent<Image>();
            _image.material.SetFloat(OpenPercentage, 0f);
            _image.enabled = false;
            subObject.SetActive(false);
        }

        public void Open()
        {
            _targetPercentage = 1f;
        }

        public void Close()
        {
            _targetPercentage = 0f;
        }

        private void Update()
        {
            var targetChange = speed * Time.unscaledDeltaTime;
            if (_targetPercentage > _currentPercentage && _targetPercentage > 0.99f)
            {
                _currentPercentage = _currentPercentage + targetChange > _targetPercentage ? _targetPercentage : _currentPercentage + targetChange;
            }
            
            if (_targetPercentage < _currentPercentage && _targetPercentage < 0.01f)
            {
                _currentPercentage = _currentPercentage - targetChange < 0f ? 0f : _currentPercentage - targetChange;
            }

            if (_currentPercentage < 0.01f)
            {
                _image.enabled = false;
            }
            else
            {
                _image.enabled = true;
            }

            if (_currentPercentage > 0.90f)
            {
                if (!_active)
                {
                    subObject.SetActive(true);
                    _active = true;
                }
            }
            else
            {
                if (_active)
                {
                    subObject.SetActive(false);
                    _active = false;
                }
            }
            
            _image.material.SetFloat(OpenPercentage, _currentPercentage);
        }

        private void OnEnable()
        {
            if (gameObject.activeSelf && openOnEnable)
            {
                Open();
            }
        }

        private void OnDisable()
        {
            Close();
            _currentPercentage = 0;
        }
    }
}