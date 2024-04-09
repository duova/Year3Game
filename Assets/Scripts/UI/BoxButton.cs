using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class BoxButton : Button
    {
        private Image _image;
        private static readonly int BackgroundAlpha = Shader.PropertyToID("_BackgroundAlpha");

        protected override void Start()
        {
            base.Start();
            TryGetComponent(out _image);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (!_image) return;
            _image.material.SetFloat(BackgroundAlpha, 0.8f);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (!_image) return;
            _image.material.SetFloat(BackgroundAlpha, 0.5f);
        }
    }
}