using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGBulletBase : BulletBase
{
    public float speed=1000.0f;
    public LayerMask layerMask;

    public BulletHitEffectSet bulletHitSet;

    public float destroyTime=1.0f;
    private float timer=0.0f;

    public override void Shoot(){
        Transform bulletTrans=Instantiate(transform,transform.position,transform.rotation) as Transform;
        Rigidbody rigidbody=bulletTrans.GetComponent<Rigidbody>();
        if(Camera.main!=null){
            Transform camTrans=Camera.main.transform;
            Ray ray=new Ray(camTrans.position,camTrans.forward);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo,1000000,layerMask)){
                bulletTrans.forward=(hitInfo.point-bulletTrans.position).normalized;
            }else{
                Vector3 targetPos=camTrans.position+camTrans.forward*1000000;
                bulletTrans.forward=(targetPos-bulletTrans.position).normalized;
            }
        }
        bulletTrans.gameObject.SetActive(true);
    }

    private void FixedUpdate() {
        Ray ray=new Ray(transform.position,transform.forward);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,speed*Time.deltaTime,layerMask)){
            transform.position=hitInfo.point;
            bulletHitSet.Play(transform.forward,hitInfo);
            Destroy(transform.gameObject);
            return;
        }else{
            transform.position+=transform.forward*speed*Time.deltaTime;
        }
        timer=timer+Time.deltaTime;
        if(timer>=destroyTime){
            Destroy(transform.gameObject);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawSphere(transform.position,0.003f);
        Gizmos.DrawRay(transform.position,transform.forward);
    }
}
