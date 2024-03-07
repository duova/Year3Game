using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HoverText : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private TMP_Text textComp;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        private void SetText(string text)
        {
            textComp.text = text;
        }

        public void Activate(string text)
        {
            Relocate((Vector2)Input.mousePosition - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + offset);
            SetText(text);
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
        
        private void Relocate(Vector2 pos)
        {
            _rectTransform.anchoredPosition = pos;
        }

        private void Update()
        {
            Relocate((Vector2)Input.mousePosition - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + offset);
        }
    }
}