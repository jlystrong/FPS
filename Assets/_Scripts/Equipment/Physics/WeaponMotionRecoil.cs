using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponMotionRecoil : EquipmentComponent
{
    [Serializable]
    public class ReloadRecoilModule{
        public RecoilForce ReloadForce = new RecoilForce();
    }
    [SerializeField]
    [Group]
    private ReloadRecoilModule[] m_ReloadRecoils;

    void Awake(){
        EquipmentAnimEventReceiver eventReceiver=GetComponentInChildren<EquipmentAnimEventReceiver>();
        eventReceiver.onAnimEvent+=OnAnimEvent;
    }

    public void OnAnimEvent(string param){
        if(param.StartsWith("Reload")){
            int reloadIndex=0;
            string s=param.Substring(6);
            try{
                reloadIndex=int.Parse(s);
            }catch(Exception e){}
            if(reloadIndex>0){
                ApplyReloadForce(reloadIndex);
            }
        }
    }

    private void ApplyReloadForce(int reloadIndex){
        if(m_ReloadRecoils.Length<=0){
            return ;
        }
        reloadIndex=(reloadIndex-1)%m_ReloadRecoils.Length;
        RecoilForce force=m_ReloadRecoils[reloadIndex].ReloadForce;
        Player.Camera.ApplyRecoil(force, 1f);
    }
}
