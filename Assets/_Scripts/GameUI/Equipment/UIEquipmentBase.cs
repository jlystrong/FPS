using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentBase : PlayerComponentFinder
{
    public GameObject moverObj;
    public GameObject touchpadObj;
    public GameObject btnFireObj;
    public GameObject btnReloadObj;
    public GameObject btnAimObj;
    

    private bool isDragging=false;

    void Awake(){
        SimpleMover mover=moverObj.GetComponent<SimpleMover>();
        mover.onMove=OnMove;

        SimpleDrag dragger=touchpadObj.GetComponent<SimpleDrag>();
        dragger.onDrag=OnDrag;
        dragger.onBeginDrag=OnBeginDrag;
        dragger.onEndDrag=OnEndDrag;
        
        SimpleButton fireButton=btnFireObj.GetComponent<SimpleButton>();
        fireButton.onDown=OnFireDown;
        fireButton.onUp=OnFireUp;
        SimpleDrag fireDragger=btnFireObj.GetComponent<SimpleDrag>();
        fireDragger.onDrag=OnDrag;
        fireDragger.onBeginDrag=OnBeginDrag;
        fireDragger.onEndDrag=OnEndDrag;
        
        SimpleButton reloadBtn=btnReloadObj.GetComponent<SimpleButton>();
        reloadBtn.onClick=OnReloadClick;

        SimpleButton aimBtn=btnAimObj.GetComponent<SimpleButton>();
        aimBtn.onClick=OnAimClick;
    }

    
    void Update(){
        if(!isDragging){
            Player.lookInput=Vector2.zero;
        }
        isDragging=false;

        if(Input.GetKeyDown(KeyCode.Space)){
            OnFireDown(null);
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            OnFireUp(null);
        }
        if(Input.GetKeyUp(KeyCode.R)){
            OnReloadClick(null);
        }
        if(Input.GetMouseButtonDown(1)){
            OnAimClick(null);
        }
    }

    private void OnMove(Vector2 moveOffset){
        Player.moveInput=moveOffset;
    }

    private void OnDrag(float x,float y){
        isDragging=true;
        Player.lookInput=new Vector2(x,y);
    }
    private void OnBeginDrag(){
        isDragging=true;
        Player.lookInput=Vector2.zero;
    }
    private void OnEndDrag(){
        isDragging=false;
        Player.lookInput=Vector2.zero;
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
