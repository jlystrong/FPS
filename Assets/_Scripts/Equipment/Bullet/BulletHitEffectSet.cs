using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletHitEffectSet : MonoBehaviour
{
    [BHeader("Scene", true)]
    public Transform defaultEffect;
    public Transform rockEffect;
    public Transform metalEffect;

    [BHeader("Zombie", true)]
    public int[] zWeight={};
    public Transform[] zTrans={};

    private int zWeightTotal=-1;

    public void Play(Vector3 forward,RaycastHit hitInfo){
        string tag=hitInfo.collider.gameObject.tag;
        if(tag.StartsWith("Z_")){
            PlayZombie(forward,hitInfo);
        }else{
            PlayScene(forward,hitInfo);
        }
    }

    public void PlayZombie(Vector3 forward,RaycastHit hitInfo){
        if(zWeightTotal<=0){
            for(int i=0;i<zWeight.Length;i++){
                zWeightTotal=zWeightTotal+zWeight[i];
            }
        }
        int seed=UnityEngine.Random.Range(0,zWeightTotal);
        Transform effectTrans=zTrans[0];
        for(int i=0;i<zWeight.Length;i++){
            seed=seed-zWeight[i];
            if(seed<0){
                effectTrans=zTrans[i];
                break;
            }
        }
        if(effectTrans!=null){
            Transform hitTrans=Instantiate(effectTrans) as Transform;
            hitTrans.position=hitInfo.point;
            hitTrans.forward=(hitInfo.normal-forward).normalized;
            hitTrans.gameObject.SetActive(true);
        }
    }

    public void PlayScene(Vector3 forward,RaycastHit hitInfo){
         string tag=hitInfo.collider.gameObject.tag;
        Transform effectTrans=defaultEffect;
        if(tag=="METAL"){
            effectTrans=metalEffect;
        }else if(tag=="ROCK"){
            effectTrans=rockEffect;
        }
        if(effectTrans!=null){
            Transform hitTrans=Instantiate(effectTrans) as Transform;
            hitTrans.position=hitInfo.point;
            hitTrans.forward=hitInfo.normal;
            hitTrans.gameObject.SetActive(true);
        }
    }
}
