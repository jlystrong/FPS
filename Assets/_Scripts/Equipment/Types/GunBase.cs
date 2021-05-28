using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : EquipmentItem
{
    public enum FireMode{
        Safety=1,
        SemiAuto=2,
        FullAtuo=3,
    }

    [BHeader("Fire Settings")]
    public FireMode Mode=FireMode.FullAtuo;
    public float FireDuration=0.25f;
    public float ReloadDuration=2.5f;


    private bool isFiring=false;
    private float m_LastFireTime=0f;

    private void Awake() {
        EquipmentAnimEventReceiver eventReceiver=GetComponentInChildren<EquipmentAnimEventReceiver>();
        eventReceiver.onAnimEvent=OnAnimEvent;
    }

    public override void FireDown(){
        isFiring=true;
    }
    public override void FireUp(){
        isFiring=false;
    }
    public override void Reload(){
        isFiring=false;
        m_Animator.SetTrigger("ToReload");
    }

    void Update(){
        if(isFiring){
            if(Time.time-m_LastFireTime>=FireDuration){
                m_Animator.SetTrigger("ToFire");
                m_LastFireTime=Time.time;
            }
        }
    }

    public void OnAnimEvent(string param){
        switch(param){
            case "Fire":OnFire();break;
            case "Reloaded":OnReloaded();break;
            default:break;
        }
    }

    public void OnFire(){
        Debug.Log("Fire");
    }
    public void OnReloaded(){
        Debug.Log("Reloaded");
    }
}
