using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityFade : MonoBehaviour
{
    public float duration = 1.0f;
    public AnimationCurve animationCurve;

    private Light _light;
    private float timer;
    private float baseIntensity;

    void Start(){
        _light=GetComponent<Light>();
        baseIntensity=_light.intensity;
    }

    void Update(){
        timer+=Time.deltaTime;
        _light.intensity=animationCurve.Evaluate(timer/duration);
        if(timer>=duration){
            Destroy(gameObject);
        }
    }
}
