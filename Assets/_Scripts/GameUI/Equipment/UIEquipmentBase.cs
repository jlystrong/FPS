using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentBase : PlayerComponentFinder
{
    public GameObject touchpadObj;
    public GameObject btnFireObj;
    public GameObject btnReloadObj;

    private float lastDragTime=0;

    void Awake(){
        SimpleDrag dragger=touchpadObj.GetComponent<SimpleDrag>();
        dragger.onDrag=OnDrag;
        dragger.onBeginDrag=OnBeginDrag;
        dragger.onEndDrag=OnEndDrag;
    }

    
    void FixedUpdate(){
        if(Time.realtimeSinceStartup-lastDragTime>=0.05f){
            Player.LookInput.Set(Vector2.zero);
        }
    }
    private void OnDrag(float x,float y){
        Player.LookInput.Set(new Vector2(x,y));
        lastDragTime=Time.realtimeSinceStartup;
    }
    private void OnBeginDrag(){
        Player.LookInput.Set(Vector2.zero);
    }
    private void OnEndDrag(){
        Player.LookInput.Set(Vector2.zero);
    }
}
