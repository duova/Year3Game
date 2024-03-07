using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    
    public class CutsceneManager : MonoBehaviour
    {
        public CutsceneManager Instance { get; private set; }

        private GameObject _cutsceneObject;

        private string _cutsceneText;

        private int _textCharacterCount;

        [SerializeField]
        private GameObject cameraArm;

        [SerializeField]
        private GameObject cameraMount;

        [SerializeField]
        private float rotationRate;

        [SerializeField]
        private TMP_Text textComp;

        [SerializeField]
        private int printRate;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayCutscene(GameObject gameObject, string text)
        {
            _cutsceneObject = gameObject;
            _cutsceneText = text;
            _textCharacterCount = 0;
            
            //Relocate camera.
            cameraArm.transform.position = _cutsceneObject.transform.position;
            var main = Camera.main;
            if (main != null)
            {
                var transform1 = main.transform;
                transform1.parent = cameraMount.transform;
                transform1.position = Vector3.zero;
                transform1.rotation = Quaternion.identity;
            }
        }

        public void OnButtonClick()
        {
            Reset();
        }

        public void Reset()
        {
            _cutsceneObject = null;
            _cutsceneText = "";
        }

        private void Update()
        {
            if (_cutsceneObject == null) return;
            var rotation = cameraArm.transform.rotation;
            rotation.eulerAngles += new Vector3(0, rotationRate, 0);
            cameraArm.transform.rotation = rotation;
        }

        private void FixedUpdate()
        {
            if (_cutsceneObject == null) return;
            if (_textCharacterCount < _cutsceneText.Length)
            {
                _textCharacterCount += printRate;
            }
            textComp.text = _cutsceneText.Substring(0, _textCharacterCount);
        }
    }
}