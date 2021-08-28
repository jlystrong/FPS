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
    public float AimViewScale=1.1f;
    public float AimSpeed=50.0f;
    public float UnaimSpeed=100.0f;
    public int continuouslyThreshold=5;

    [Serializable]
    public struct EquipmentSettings{
        public BulletBase bullet;
        public Transform MuzzleEffect;
        public Transform MuzzleEndEffect;
        public LightEffect[] LightEffects;
    }
    [SerializeField]
    [Group]
    private EquipmentSettings m_EquipmentSettings = new EquipmentSettings();

    [Serializable]
    public struct EquipmentSound{
        public GameSoundPackFlat shootPack;
        public GameSoundPackFlat shootTrailPack;
    }
    [SerializeField]
    [Group]
    private EquipmentSound m_EquipmentSound = new EquipmentSound();

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
        if(Player.isReloading){
            return ;
        }
        Player.isFiring=true;
        continuouslyUsedTimes=0;
        if(Time.time-m_LastFireTime>=FireDuration){
            if(Player.startFireAction!=null){
                Player.startFireAction();
            } 
        }
    }
    public override void FireUp(){
        if(Player.isFiring){
            if(Player.endFireAction!=null){
                Player.endFireAction();
            }
            OnFireEnd();
        }
        continuouslyUsedTimes=0;
        Player.isFiring=false;
    }
    public override void Reload(){
        if(Player.isReloading){
            return ;
        }
        Player.isFiring=false;
        Player.isReloading=true;
        m_Animator.SetTrigger("ToReload");

        Player.isAiming=false;
        m_Animator.SetFloat("Aim",0);
    }
    public override void Aim(){
        if(Player.isReloading){
            return ;
        }
        if(Player.isAiming){
            Player.Camera.SetViewScale(1,UnaimSpeed);
            Player.isAiming=false;
            m_Animator.SetFloat("Aim",0);
        }else{
            Player.Camera.SetViewScale(AimViewScale,AimSpeed);
            Player.isAiming=true;
            m_Animator.SetFloat("Aim",1);
        }
    }

    void Update(){
        if(Player.isFiring){
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
        Player.isReloading=false;
        continuouslyUsedTimes=continuouslyUsedTimes+1;
        if(Player.fireAction!=null){
            Player.fireAction(true);
        }
        for(int i=0;i<m_EquipmentSettings.LightEffects.Length;i++){
            m_EquipmentSettings.LightEffects[i].Play(false);
        }
        Transform muzzleEffectRootTrans=m_EquipmentSettings.MuzzleEffect.parent;
        Transform muzzleEffectTrans=Instantiate(m_EquipmentSettings.MuzzleEffect);
        muzzleEffectTrans.SetParent(muzzleEffectRootTrans,false);
        muzzleEffectTrans.gameObject.SetActive(true);

        m_EquipmentSettings.bullet.Shoot();

        m_EquipmentSound.shootPack.Play();
    }
    public void OnFireEnd(){
        if(continuouslyUsedTimes>=continuouslyThreshold){
            Transform muzzleEffectRootTrans=m_EquipmentSettings.MuzzleEffect.parent;
            Transform muzzleEffectTrans=Instantiate(m_EquipmentSettings.MuzzleEndEffect);
            muzzleEffectTrans.SetParent(muzzleEffectRootTrans,false);
            muzzleEffectTrans.gameObject.SetActive(true);

            m_EquipmentSound.shootTrailPack.Play();
        }
    }
    public void OnFireAnim(){
        // Debug.Log("FireAnim");
    }
    public void OnReloadedAnim(){
        Debug.Log("Reloaded");
        Player.isReloading=false;
    }
}
