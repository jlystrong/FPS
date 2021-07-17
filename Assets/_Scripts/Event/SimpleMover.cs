using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleMover : MonoBehaviour
{
    public UITrigger trigger;
    public GameObject moverObj;
    public RectTransform moverTrans;
    public RectTransform dotTrans;
    public float maxR=100f;
    public float sensitive=1f;

    public System.Action<Vector2> onMove;
    public System.Action onBeginDrag;
    public System.Action onEndDrag;

    private bool isMoving=false;
    private Vector2 moveOffset=Vector2.zero;
    private Vector2 startPos=Vector3.zero;
    private Vector2 posOffset=Vector3.zero;

    private void Awake() {
        moverObj.SetActive(false);
        trigger.OnPointerDownAction=OnPointerDown;
        trigger.OnPointerUpAction=OnPointerUp;
        trigger.OnDragAction=OnDrag;
    }

    public void OnPointerDown(PointerEventData eventData){
        isMoving=true;
        startPos=eventData.pressPosition;
        moverObj.gameObject.SetActive(true);
        moverTrans.anchoredPosition=startPos;
        posOffset=Vector2.zero;
        ResetMover();
    }
    public void OnPointerUp(PointerEventData eventData){
        isMoving=false;
        moverObj.SetActive(false);
        posOffset=Vector2.zero;
        ResetMover();
    }

    public void OnDrag(PointerEventData eventData){
        Vector2 delta=eventData.delta;
        posOffset.x=posOffset.x+delta.x;
        posOffset.y=posOffset.y+delta.y;
        ResetMover();
    }
    public void OnBeginDrag(PointerEventData eventData){
        if(onBeginDrag!=null){
            onBeginDrag();
        }
    }
    public void OnEndDrag(PointerEventData eventData){
        if(onEndDrag!=null){
            onEndDrag();
        }
    }

    private void ResetMover(){
        Vector2 dotPos=posOffset*sensitive;
        float mag=posOffset.magnitude;
        if(mag>maxR){
            dotPos=dotPos*maxR/mag;
        }
        dotTrans.anchoredPosition=dotPos;
        moveOffset=dotPos/maxR;
    }

    private void Update() {
        if(onMove!=null){
            if(isMoving){
                onMove(moveOffset);
            }else{
                onMove(Vector2.zero);
            }
            }
    }
}
