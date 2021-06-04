using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float delay=0f;

    void OnEnable(){
        Destroy(gameObject,delay);
    }
}
