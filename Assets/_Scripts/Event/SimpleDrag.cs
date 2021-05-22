using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleDrag : EventTrigger
{
    public System.Action<float,float> onDrag;
    public System.Action onBeginDrag;
    public System.Action onEndDrag;

    public override void OnDrag(PointerEventData eventData){
        if(onDrag!=null){
            Vector2 delta=eventData.delta;
            onDrag(delta.x,delta.y);
        }
    }
    public override void OnBeginDrag(PointerEventData eventData){
        if(onBeginDrag!=null){
            onBeginDrag();
        }
    }
    public override void OnEndDrag(PointerEventData eventData){
        if(onEndDrag!=null){
            onEndDrag();
        }
    }
}
