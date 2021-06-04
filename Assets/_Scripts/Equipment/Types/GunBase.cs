using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunBase : EquipmentItem
{
    public enum FireMode{
        Safety=1,
        SemiAuto=2,
        FullAtuo=3,
    }
    [BHeader("Fire Settings")]
    public FireMode Mode=FireMode.FullAtuo;
    public float FireDuration=0.1f;
    public float ReloadDuration=2.5f;

    [Serializable]
    public struct EquipmentSettings{
        public Transform MuzzleEffect;
        public Transform MuzzleEndEffect;
        public LightEffect LightEffect;
    }
    [SerializeField]
    [Group]
    private EquipmentSettings m_EquipmentSettings = new EquipmentSettings();

    [Serializable]
    public struct TransformSettings{
        public Vector3 position;
        public Vector3 rotation;
    }
    [SerializeField]
    [Group]
    private TransformSettings m_AimSettings = new TransformSettings();

    private bool isFiring=false;
    private bool isReloading=false;
    private float m_LastFireTime=0f;

    private void Awake() {
        EquipmentAnimEventReceiver eventReceiver=GetComponentInChildren<EquipmentAnimEventReceiver>();
        eventReceiver.onAnimEvent+=OnAnimEvent;

        AnimatorOverrideController ac = m_Animator.runtimeAnimatorController as AnimatorOverrideController;
        AnimationClipPair[] clipPairs =  ac.clips;
        for(int i=0;i<clipPairs.Length;i++){
            if(clipPairs[i].originalClip.name=="Fire"){
                float origLength=clipPairs[i].originalClip.length;
                m_Animator.SetFloat("FireSpeed",Mathf.Pow(origLength/FireDuration,0.3f));
            }else if(clipPairs[i].originalClip.name=="Reload"){
                float origLength=clipPairs[i].originalClip.length;
                m_Animator.SetFloat("ReloadSpeed",origLength/ReloadDuration);
            }
        }
    }

    public override void FireDown(){
        isFiring=true;
        continuouslyUsedTimes=0;
        if(Time.time-m_LastFireTime>=FireDuration){
            if(Player.startFireAction!=null){
                Player.startFireAction();
            } 
        }
    }
    public override void FireUp(){
        if(isFiring){
            if(Player.endFireAction!=null){
                Player.endFireAction();
            }
            OnFireEnd();
        }
        continuouslyUsedTimes=0;
        isFiring=false;
    }
    public override void Reload(){
        isFiring=false;
        isReloading=true;
        m_Animator.SetTrigger("ToReload");
    }
    public override void Aim(){
        if(isReloading){
            return ;
        }
        if(Player.Aim.Active){
            Player.Aim.ForceStop();
            m_Animator.SetFloat("Aim",0);
        }else{
            Player.Aim.ForceStart();
            m_Animator.SetFloat("Aim",1);
        }
    }

    void Update(){
        if(isFiring){
            if(Time.time-m_LastFireTime>=FireDuration){
                m_LastFireTime=Time.time;
                m_Animator.SetTrigger("ToFire");
                OnFire();
            }
        }
    }

    public void OnAnimEvent(string param){
        switch(param){
            case "Fire":OnFireAnim();break;
            case "Reloaded":OnReloadedAnim();break;
            default:break;
        }
    }

    public void OnFire(){
        // Debug.Log("Fire");
        isReloading=false;
        continuouslyUsedTimes=continuouslyUsedTimes+1;
        if(Player.fireAction!=null){
            Player.fireAction(true);
        }
        m_EquipmentSettings.LightEffect.Play(false);
        Transform muzzleEffectRootTrans=m_EquipmentSettings.MuzzleEffect.parent;
        Transform muzzleEffectTrans=Instantiate(m_EquipmentSettings.MuzzleEffect);
        muzzleEffectTrans.SetParent(muzzleEffectRootTrans,false);
        muzzleEffectTrans.gameObject.SetActive(true);
    }
    public void OnFireEnd(){
        if(continuouslyUsedTimes>=3){
            Transform muzzleEffectRootTrans=m_EquipmentSettings.MuzzleEffect.parent;
            Transform muzzleEffectTrans=Instantiate(m_EquipmentSettings.MuzzleEndEffect);
            muzzleEffectTrans.SetParent(muzzleEffectRootTrans,false);
            muzzleEffectTrans.gameObject.SetActive(true);
        }
    }
    public void OnFireAnim(){
        // Debug.Log("FireAnim");
    }
    public void OnReloadedAnim(){
        Debug.Log("Reloaded");
        isReloading=false;
    }
}
