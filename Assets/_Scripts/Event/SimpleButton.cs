using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleButton : EventTrigger
{
    public System.Action<Transform> onClick;
    public System.Action<Transform> onDown;
    public System.Action<Transform> onUp;

    public override void OnPointerClick(PointerEventData eventData){
        if(onClick!=null){
            onClick(transform);
        }
    }
    public override void OnPointerDown(PointerEventData eventData){
        if(onDown!=null){
            onDown(transform);
        }
    }
    public override void OnPointerUp(PointerEventData eventData){
        if(onUp!=null){
            onUp(transform);
        }
    }
}
