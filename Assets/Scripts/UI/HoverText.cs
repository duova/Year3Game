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

        [SerializeField] private Canvas canvas;

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
            Relocate();
            SetText(text);
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            Relocate();
        }

        private void Relocate()
        {
            var scaleFactor = canvas.scaleFactor;
            var clampedMousePos = new Vector2(Mathf.Clamp(Input.mousePosition.x, 150 * scaleFactor, Screen.width - 150 * scaleFactor),
                Mathf.Clamp(Input.mousePosition.y, 150 * scaleFactor, Screen.height - 150 * scaleFactor));
            _rectTransform.anchoredPosition = (clampedMousePos - new Vector2(Screen.width, Screen.height) / 2f) / scaleFactor + offset;
        }
    }
}