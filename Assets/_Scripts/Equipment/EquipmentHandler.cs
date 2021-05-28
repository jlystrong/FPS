using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipmentHandler : PlayerComponent
{
    [Serializable]
    protected struct BaseSettings{
        public Animator AnimController;
    }

    [SerializeField]
    [Group]
    protected BaseSettings m_BaseSettings = new BaseSettings();

    private Transform m_Pivot;

    protected EquipmentItem m_CurrentItem;
    public EquipmentItem CurrentItem { get { return m_CurrentItem; } private set { } }
    public Transform ItemTransform { get { return m_CurrentItem != null ? m_CurrentItem.transform : transform; } private set { } }
    public System.Action<bool> OnSelected=null;

    void Awake() {
         m_Pivot=transform.Find("Offset");
    }

    void Start(){
        ChangeItem(m_Pivot.Find("M1A"));
    }

    public void ChangeItem(Transform newItem){
        newItem.SetParent(m_Pivot,false);
        newItem.localPosition=Vector3.zero;
        newItem.localEulerAngles=Vector3.zero;

        m_CurrentItem=newItem.GetComponent<EquipmentItem>();
        Player.m_CurrentItem=m_CurrentItem;
        m_BaseSettings.AnimController=m_CurrentItem.m_Animator;

        OnSelected(true);
    }
}
