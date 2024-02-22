using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public delegate void OnUpEvent(AddedEventsButton button);
    
    public delegate void OnHoverStateEvent(AddedEventsButton button);
    
    public class AddedEventsButton : Button
    {
        public OnUpEvent OnUp;

        public OnHoverStateEvent OnEnter;
        
        public OnHoverStateEvent OnExit;
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnUp.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnEnter.Invoke(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnExit.Invoke(this);
        }
    }
}