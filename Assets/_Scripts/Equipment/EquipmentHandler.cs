using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipmentHandler : PlayerComponent
{
    [Serializable]
    protected struct BaseSettings{
        public Animator AnimController;
        // public Transform AccesoryParent;
        // [Space]
        // public SkinnedMeshRenderer Item;
        // public SkinnedMeshRenderer RightArm;
        // public SkinnedMeshRenderer LeftArm;
    }
    [SerializeField]
    [Group]
    protected BaseSettings m_BaseSettings = new BaseSettings();

    [Serializable]
    public struct EquipmentSettings{
        public Vector3 OriginalLightPosition { get; set; }
        // public Transform Armature;
        public Transform Muzzle;
        // public Transform CasingEjection;
        // public Transform MagazineEjection;
        // public Transform[] BulletBones;
        // [Space]
        // public LightEffect LightEffect;
    }
    [SerializeField]
    [Group]
    private EquipmentSettings m_EquipmentSettings = new EquipmentSettings();

    //Event
    public System.Action<bool> OnSelected=null;

    //Properties
    public EquipmentPhysicsHandler PhysicsHandler { get { return m_ItemPhysicsHandler; } private set { } }
    // public EquipmentManager EquipmentManager { get { return m_EquipmentManager; } private set { } }
    // public SaveableItem CurrentlyAttachedItem { get { return m_CurrentlyAttachedItem; } private set { } }
    public EquipmentItem CurrentItem { get { return m_CurrentItem; } private set { } }
    // public GameObject ItemModelTransform { get => m_BaseSettings.Item.gameObject; private set { } }
    public Transform ItemTransform { get { return m_CurrentItem != null ? m_CurrentItem.transform : transform; } private set { } }
    public Animator Animator { get { return m_BaseSettings.AnimController; } private set { } }
    public float LastChangeItemTime { get; private set; }

    private EquipmentPhysicsHandler m_ItemPhysicsHandler;
    protected EquipmentItem m_CurrentItem;

    // private List<QueuedCameraForce> m_QueuedCamForces = new List<QueuedCameraForce>();
    // public void ClearDelayedCamForces() { m_QueuedCamForces.Clear(); }

    // public bool TryUseOnce(Camera camera){
    //     bool usedSuccessfully = m_CurrentItem.TryUseOnce(camera);
    //     if (usedSuccessfully){
    //         UsingItem.ForceStart();
    //         CurrentItem.OnUseStart();
    //     }
    //     ItemUsed.Send(usedSuccessfully, false);
    //     return usedSuccessfully;
    // }
    // public bool TryUseContinuously(Camera camera){
    //     bool usedSuccessfully = m_CurrentItem.TryUseContinuously(camera);
    //     if (usedSuccessfully){
    //         UsingItem.ForceStart();
    //         CurrentItem.OnUseStart();
    //     }
    //     ItemUsed.Send(usedSuccessfully, true);
    //     return usedSuccessfully;
    // }

    void Awake(){
        m_ItemPhysicsHandler = GetComponent<EquipmentPhysicsHandler>();
        // Equip(transform.Find("Offset/M1A"));
        Equip(transform.Find("Offset/M1911"));
    }
    public void Equip(Transform itemTrans){
        if (m_CurrentItem != null){
            // m_CurrentItem.gameObject.SetActive(false);
            Destroy(m_CurrentItem.gameObject);
        }

        m_CurrentItem=itemTrans.GetComponent<EquipmentItem>();
        Player.m_CurrentItem=m_CurrentItem;
        m_BaseSettings.AnimController=m_CurrentItem.m_Animator;
        EquipmentComponent[] equipComps = m_CurrentItem.GetComponents<EquipmentComponent>();
        foreach (var comp in equipComps){
            comp.Initialize(this);
        }
        // m_QueuedCamForces.Clear();
        LastChangeItemTime = Time.time;

        m_CurrentItem.gameObject.SetActive(true);
        m_CurrentItem.Equip();

        OnSelected(true);
    }
    public void UnwieldItem(){
        m_CurrentItem.Unequip();
        OnSelected(false);
    }

    void Update(){
        // for (int i = 0; i < m_QueuedCamForces.Count; i++){
        //     if (Time.time >= m_QueuedCamForces[i].PlayTime){
        //         var force = m_QueuedCamForces[i].DelayedForce.Force;
        //         Player.Camera.AddRotationForce(force.Force, force.Distribution);
        //         m_QueuedCamForces.RemoveAt(i);
        //     }
        // }
    }

    // public void PlayCameraForce(DelayedCameraForce delayedCamForce){
    //     m_QueuedCamForces.Add(new QueuedCameraForce(delayedCamForce, Time.time + delayedCamForce.Delay));
    // }
    // public void PlayCameraForces(DelayedCameraForce[] delayedForces){
    //     for (int i = 0; i < delayedForces.Length; i++)
    //         PlayCameraForce(delayedForces[i]);
    // }
}
