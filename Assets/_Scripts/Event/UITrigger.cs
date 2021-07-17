using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITrigger : EventTrigger
{
    public System.Action<PointerEventData> OnBeginDragAction;
    public override void OnBeginDrag(PointerEventData eventData){
        if(OnBeginDragAction!=null)
            OnBeginDragAction(eventData);
    }
    public System.Action<PointerEventData> OnDragAction;
    public override void OnDrag(PointerEventData eventData){
        if(OnDragAction!=null)
            OnDragAction(eventData);
    }
    public System.Action<PointerEventData> OnEndDragAction;
    public override void OnEndDrag(PointerEventData eventData){
        if(OnEndDragAction!=null)
            OnEndDragAction(eventData);
    }
    public System.Action<AxisEventData> OnMoveAction;
    public override void OnMove(AxisEventData eventData){
        if(OnMoveAction!=null)
            OnMoveAction(eventData);
    }
    public System.Action<PointerEventData> OnPointerClickAction;
    public override void OnPointerClick(PointerEventData eventData){
        if(OnPointerClickAction!=null)
            OnPointerClickAction(eventData);
    }
    public System.Action<PointerEventData> OnPointerDownAction;
    public override void OnPointerDown(PointerEventData eventData){
        if(OnPointerDownAction!=null)
            OnPointerDownAction(eventData);
    }
    public System.Action<PointerEventData> OnPointerUpAction;
    public override void OnPointerUp(PointerEventData eventData){
        if(OnPointerUpAction!=null)
            OnPointerUpAction(eventData);
    }
}
