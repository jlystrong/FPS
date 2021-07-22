using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISightBase : PlayerComponentFinder
{
    [SerializeField]
    private RectTransform[] crossTranss1;
    [SerializeField]
    private RectTransform[] crossTranss2;
    [SerializeField]
    private float startFireForce=0.1f;
    [SerializeField]
    private float endFireForce=0.1f;
    [SerializeField]
    private float fireForce=0.1f;
    [SerializeField]
    private float lookForce=0.1f;
    [SerializeField]
    private float unaimForce=0.5f;
    [SerializeField]
    private float maxForce=1.0f;
    [SerializeField]
    private float downForce=1.0f;
    [SerializeField]
    private float forceToScale=1.0f;

    private Vector2[] origSizes1=null;
    private Vector2[] origSizes2=null;

    private float currentForce=0;
    private bool isAiming=false;

    void Awake(){
        origSizes1=new Vector2[crossTranss1.Length];
        for(int i=0;i<crossTranss1.Length;i++){
            origSizes1[i]=crossTranss1[i].sizeDelta;
        }
        origSizes2=new Vector2[crossTranss2.Length];
        for(int i=0;i<crossTranss2.Length;i++){
            origSizes2[i]=crossTranss2[i].sizeDelta;
        }
    }
    private void OnEnable(){
        currentForce=0;
        UnAim();
        Player.startFireAction+=StartFire;
        Player.endFireAction+=EndFire;
        Player.fireAction+=Fire;
    }
    private void OnDisable() {
        Player.startFireAction-=StartFire;
        Player.endFireAction-=EndFire;
        Player.fireAction-=Fire;
    }

    private void StartFire(){
        ApplyForce(startFireForce);
    }
    private void EndFire(){
        ApplyForce(endFireForce);
    }
    private void Fire(bool continuously){
        ApplyForce(fireForce);
    }

    public void Aim(){
        isAiming=true;
        for(int i=0;i<crossTranss1.Length;i++){
            crossTranss1[i].gameObject.SetActive(false);
        }
        for(int i=0;i<crossTranss2.Length;i++){
            crossTranss2[i].gameObject.SetActive(false);
        }
    }
    public void UnAim(){
        isAiming=false;
        for(int i=0;i<crossTranss1.Length;i++){
            crossTranss1[i].gameObject.SetActive(true);
        }
        for(int i=0;i<crossTranss2.Length;i++){
            crossTranss2[i].gameObject.SetActive(true);
        }
        ApplyForce(unaimForce);
    }

    public void ApplyForce(float force){
        currentForce+=force;
        if(currentForce>maxForce){
            currentForce=maxForce;
        }
    }

    void Update(){
        currentForce+=Player.lookInput.magnitude*Time.deltaTime*lookForce;
        if(currentForce>maxForce){
            currentForce=maxForce;
        }
        float scaleAdd=currentForce*forceToScale;
        for(int i=0;i<crossTranss1.Length;i++){
            crossTranss1[i].sizeDelta=origSizes1[i]*new Vector2(1+scaleAdd,1);
        }
        for(int i=0;i<crossTranss2.Length;i++){
            crossTranss2[i].sizeDelta=origSizes2[i]*new Vector2(1,1+scaleAdd);
        }
        currentForce-=Time.deltaTime*downForce;
        if(currentForce<0){
            currentForce=0;
        }
        if(Player.isAiming && !isAiming){
            Aim();
        }else if(!Player.isAiming && isAiming){
            UnAim();
        }
    }
}
