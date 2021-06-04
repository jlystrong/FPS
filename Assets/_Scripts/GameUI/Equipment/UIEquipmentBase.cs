using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentBase : PlayerComponentFinder
{
    public GameObject touchpadObj;
    public GameObject btnFireObj;
    public GameObject btnReloadObj;
    public GameObject btnAimObj;

    private bool isDragging=false;

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

        SimpleButton aimBtn=btnAimObj.GetComponent<SimpleButton>();
        aimBtn.onClick=OnAimClick;
    }

    
    void Update(){
        if(!isDragging){
            Player.LookInput.Set(Vector2.zero);
        }
        isDragging=false;
    }
    private void OnDrag(float x,float y){
        isDragging=true;
        Player.LookInput.Set(new Vector2(x,y));
    }
    private void OnBeginDrag(){
        isDragging=true;
        Player.LookInput.Set(Vector2.zero);
    }
    private void OnEndDrag(){
        isDragging=false;
        Player.LookInput.Set(Vector2.zero);
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
    private void OnAimClick(Transform btnTrans){
        Player.m_CurrentItem.Aim();
    }
}
