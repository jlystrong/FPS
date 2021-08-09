using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTextureAnimator : MonoBehaviour
{
    public Renderer[] AnimatedRenderers = null;
    public string propertyName = "_MainTex";
    public int Rows = 4;
    public int Columns = 4;
    public float Duration = 1.0f;
    public float DurationOffset = 0.0f;
    public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);
    public bool IsLoop = true;
    public bool IsReverse = false;

    private float timer=0f;

    private void OnEnable() {
        timer=0f;
        Duration=Duration+DurationOffset*Random.Range(-1.0f,1.0f);
        UpdateMaterial();
    }

    private void Update() {
        timer+=Time.deltaTime;
        UpdateMaterial();
    }

    private void UpdateMaterial(){
        Vector2 size=new Vector2(1f / Columns, 1f / Rows);
        float alpha=timer/Duration;
        alpha=Mathf.Clamp01(animationCurve.Evaluate(alpha));
        int count=Rows*Columns;
        int index=Mathf.RoundToInt(alpha*count);
        if(index>=count){
            index=count-1;
        }
        if(IsReverse){
            index=count-1-index;
        }
        Vector2 offset = new Vector2((float) index / Columns - (index / Columns), 1 - (index / Columns) / (float) Rows);
        for(int i=0;i<AnimatedRenderers.Length;i++){
            AnimatedRenderers[i].material.SetTextureScale(propertyName,size);
            AnimatedRenderers[i].material.SetTextureOffset(propertyName,offset);
        }
        if(timer>=Duration){
            if(IsLoop){
                timer=timer-Duration;
            }else{
                for(int i=0;i<AnimatedRenderers.Length;i++){
                    AnimatedRenderers[i].gameObject.SetActive(false);
                }
                enabled=false;
            }
        }
    }
}
