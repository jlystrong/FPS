using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : PlayerComponentFinder
{
    public GameObject touchpadObj;
    public GameObject btnFireObj;
    public GameObject btnReloadObj;


    void Awake(){
        SimpleDrag dragger=touchpadObj.GetComponent<SimpleDrag>();
        dragger.onDrag=OnDrag;
        dragger.onBeginDrag=OnBeginDrag;
        dragger.onEndDrag=OnEndDrag;
    }

    private void OnDrag(float x,float y){
        Player.LookInput.Set(new Vector2(x,y));
    }
    private void OnBeginDrag(){
        Player.LookInput.Set(Vector2.zero);
    }
    private void OnEndDrag(){
        Player.LookInput.Set(Vector2.zero);
    }
}
