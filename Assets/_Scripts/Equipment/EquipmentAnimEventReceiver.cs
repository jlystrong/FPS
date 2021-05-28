using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentAnimEventReceiver : MonoBehaviour
{
    public System.Action<string> onAnimEvent;

    public void OnEvent(string eventName){
        if(onAnimEvent!=null){
            onAnimEvent(eventName);
        }
    }
}
