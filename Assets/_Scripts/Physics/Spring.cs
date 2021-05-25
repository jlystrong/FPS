using System;
using UnityEngine;

public class Spring
{
    private Transform m_Transform;
    private Vector3 m_OrigPosition;
    private Quaternion m_OrigRotation;

    public Spring(Transform transform){
        m_Transform = transform;
        m_OrigPosition=transform.localPosition;
        m_OrigRotation=transform.localRotation;
    }
    public void AddPosition(Vector3 offset){
        m_Transform.localPosition+=offset;
    }
    public void AddRotation(Vector3 offset){
        m_Transform.localEulerAngles+=offset;
    }

    public void Update(float posLerpSpeed,float rotLerpSpeed){
        Vector3 pos=m_Transform.localPosition;
        pos=Vector3.Lerp(pos,m_OrigPosition,Time.deltaTime*posLerpSpeed);
        m_Transform.localPosition=pos;

        Quaternion rot=m_Transform.localRotation;
        rot=Quaternion.Slerp(rot,m_OrigRotation,Time.deltaTime*rotLerpSpeed);
        m_Transform.localRotation=rot;
    }
}