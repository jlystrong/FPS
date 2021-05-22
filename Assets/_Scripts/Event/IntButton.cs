using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntButton : EventTrigger
{
    public int number=0;
    public System.Action<int,Transform> onClick;
    public override void OnPointerClick(PointerEventData eventData){
        if(onClick!=null){
            onClick(number,transform);
        }
    }
}
