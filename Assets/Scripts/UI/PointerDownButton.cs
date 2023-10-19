using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public delegate void OnDownEvent(PointerDownButton button);
    
    public class PointerDownButton : Button
    {
        public OnDownEvent OnDown;
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnDown.Invoke(this);
        }
    }
}