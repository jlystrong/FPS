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
        
        SimpleButton fireButton=btnFireObj.GetComponent<SimpleButton>();
        fireButton.onDown=OnFireDown;
        fireButton.onUp=OnFireUp;
        
        SimpleButton reloadBtn=btnReloadObj.GetComponent<SimpleButton>();
        reloadBtn.onClick=OnReloadClick;
    }

    
    void Update(){
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
        lastDragTime=Time.realtimeSinceStartup;
    }
    private void OnEndDrag(){
        Player.LookInput.Set(Vector2.zero);
        lastDragTime=0;
    }

    private void OnFireDown(Transform btnTrans){
        Player.m_CurrentItem.FireDown();
    }
    private void OnFireUp(Transform btnTrans){
        Player.m_CurrentItem.FireUp();
    }
    private void OnReloadClick(Transform btnTrans){
        Player.m_CurrentItem.Reload();
    }
}
