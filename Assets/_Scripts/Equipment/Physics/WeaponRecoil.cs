using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponRecoil : EquipmentComponent
{
    [Serializable]
    public class WeaponRecoilModule{
        public float ChangeFireModeInterval = 1f;
        public AnimationCurve RecoilOverTime = null;
        [Space]
        public Spring.Data PositionSpring = Spring.Data.Default;
        public Spring.Data RotationSpring = Spring.Data.Default;
        [Space]
        [Group]
        public RecoilForce ShootForce = new RecoilForce();
        [Group]
        public RecoilForce AimShootForce = new RecoilForce();
        [Group]
        public RecoilForce ChangeFireModeForce = new RecoilForce();
    }
    [SerializeField]
    [Group]
    private WeaponRecoilModule m_WeaponModelRecoil = null;

    [Serializable]
    public class CameraRecoilModule{
        [SerializeField]
        [Range(0f, 2f)]
        public float AimMultiplier = 0.75f;
        [BHeader("Controllable recoil", order = 100)]
        public float RecoilPatternMultiplier = 1f;
        public Vector2[] RecoilPattern;
        public Easings.Function RecoilControlEasing = Easings.Function.QuadraticEaseInOut;
        public float RecoilControlDelay = 1f;
        public float RecoilControlDuration = 3f;
        [Space(3f)]
        [BHeader("Non-Controllable recoil", order = 100)]
        public Spring.Data SpringData = Spring.Data.Default;
        public RecoilForce ShootForce = new RecoilForce();
        public CameraShakeSettings ShootShake = null;
    }
    [SerializeField]
    [Group]
    private CameraRecoilModule m_CameraRecoil = null;

    private EquipmentItem m_AttacheEquipment;

    private bool m_ChangedSpring;
    private bool m_AdditiveRecoilActive;
    private bool m_RecoilControlActive;
    private Easer m_RecoilControlLerper;

    private float m_RecoilStartTime;
    private Vector2 m_RecoilToAdd;
    private Vector2 m_RecoilFrameRemove;
    private float m_LastChangeFireModeTime;

    void OnEnable(){
        Player.startFireAction+=ChangeFireForce;
        Player.endFireAction+=ChangeFireForce;
        Player.fireAction+=AddImpulseRecoil;

        m_AttacheEquipment = GetComponent<EquipmentItem>();
    }
    void OnDisable(){
        Player.startFireAction-=ChangeFireForce;
        Player.endFireAction-=ChangeFireForce;
        Player.fireAction-=AddImpulseRecoil;
    }

    private void AddImpulseRecoil(bool continuously){
        if ((m_AttacheEquipment.ContinuouslyUsedTimes == 1 || !m_ChangedSpring) && !Player.Aim.Active){
            m_EHandler.PhysicsHandler.PositionSpring.Adjust(m_WeaponModelRecoil.PositionSpring);
            m_EHandler.PhysicsHandler.RotationSpring.Adjust(m_WeaponModelRecoil.RotationSpring);
            m_ChangedSpring = true;
        }
        float recoilMultiplier = m_WeaponModelRecoil.RecoilOverTime.Evaluate(m_AttacheEquipment.ContinuouslyUsedTimes / (float)m_AttacheEquipment.TryGetMagazineSize());
        if (Player.Aim.Active)
            m_WeaponModelRecoil.AimShootForce.PlayRecoilForce(recoilMultiplier, m_EHandler.PhysicsHandler.RotationSpring, m_EHandler.PhysicsHandler.PositionSpring);
        else
            m_WeaponModelRecoil.ShootForce.PlayRecoilForce(recoilMultiplier, m_EHandler.PhysicsHandler.RotationSpring, m_EHandler.PhysicsHandler.PositionSpring);

        Player.Camera.ApplyRecoil(m_CameraRecoil.ShootForce, Player.Aim.Active ? m_CameraRecoil.AimMultiplier : 1f);
        if (m_CameraRecoil.ShootShake.PositionAmplitude != Vector3.zero && m_CameraRecoil.ShootShake.RotationAmplitude != Vector3.zero){
            Player.Camera.DoShake(m_CameraRecoil.ShootShake, 1f);
        }
    }
    // private void Update(){
    //     if (m_CameraRecoil.RecoilPattern.Length == 0){
    //         return;
    //     }
    // }

    private void ChangeFireForce(){
        if(m_LastChangeFireModeTime+m_WeaponModelRecoil.ChangeFireModeInterval<=Time.time && m_AttacheEquipment.ContinuouslyUsedTimes>=3){
            m_LastChangeFireModeTime=Time.time;
            float forceMul=1f;
            if(Player.Aim.Active){
                forceMul=0.25f;
            }
            m_WeaponModelRecoil.ChangeFireModeForce.PlayRecoilForce(forceMul, m_EHandler.PhysicsHandler.RotationSpring, m_EHandler.PhysicsHandler.PositionSpring);
        }
    }
}
