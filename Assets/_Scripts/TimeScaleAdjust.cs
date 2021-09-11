using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TimeScaleAdjust : MonoBehaviour
{
    public float timeScale=1;
    // Update is called once per frame
    void Update()
    {
        Time.timeScale=timeScale;
    }
}
