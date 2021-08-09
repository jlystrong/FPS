using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
	private Camera m_Camera = null;

    [SerializeField]
    private float baseFov = 50.0f;
    private float targetFov = 50.0f;
    private float fovLerpSpeed = 10.0f;

    [Serializable]
	public struct SpringSettings{
		public static SpringSettings Default { get { return new SpringSettings() { Position = Spring.Data.Default, Rotation = Spring.Data.Default }; } }
		public Spring.Data Position, Rotation;
	}
    [Serializable]
    private class SpringsModule{
        public float SpringLerpSpeed = 25f;
        [Space]
        [Group]
        public SpringSettings ForceSprings = SpringSettings.Default;
        [Group]
        public SpringSettings HeadbobSprings = SpringSettings.Default;
    }
    [Space]
    [SerializeField, Group()]
	private SpringsModule m_Springs = null;

    [Serializable]
    private struct ShakesModule{
        public Spring.Data ShakeSpringSettings;
        public CameraShakeSettings ExplosionShake;
        public CameraShakeSettings DeathShake;
    }
    [SerializeField, Group()]
	private ShakesModule m_CamShakes = new ShakesModule();

    private List<CameraShake> m_Shakes = new List<CameraShake>();

    // Springs
	private Spring m_PositionSpring_Force;
	private Spring m_RotationSpring_Force;
    private Spring m_PositionShakeSpring;
    private Spring m_RotationShakeSpring;
    private Spring m_PositionRecoilSpring;
    private Spring m_RotationRecoilSpring;

    void Awake(){
        // Force Springs
        m_PositionSpring_Force = new Spring(Spring.Type.OverrideLocalPosition, m_Camera.transform);
        m_PositionSpring_Force.Adjust(m_Springs.ForceSprings.Position.Stiffness, m_Springs.ForceSprings.Position.Damping);
        m_RotationSpring_Force = new Spring(Spring.Type.OverrideLocalRotation, m_Camera.transform);
        m_RotationSpring_Force.Adjust(m_Springs.ForceSprings.Rotation.Stiffness, m_Springs.ForceSprings.Rotation.Damping);
        m_PositionSpring_Force.LerpSpeed = m_Springs.SpringLerpSpeed;
        m_RotationSpring_Force.LerpSpeed = m_Springs.SpringLerpSpeed;

        // Shake Springs
        m_PositionShakeSpring = new Spring(Spring.Type.AddToLocalPosition, m_Camera.transform);
        m_PositionShakeSpring.Adjust(m_CamShakes.ShakeSpringSettings);
        m_RotationShakeSpring = new Spring(Spring.Type.AddToLocalRotation, m_Camera.transform);
        m_RotationShakeSpring.Adjust(m_CamShakes.ShakeSpringSettings);
        m_PositionShakeSpring.LerpSpeed = m_Springs.SpringLerpSpeed;
        m_RotationShakeSpring.LerpSpeed = m_Springs.SpringLerpSpeed;

        //Recoil Springs
        m_PositionRecoilSpring = new Spring(Spring.Type.AddToLocalPosition, m_Camera.transform);
        m_PositionRecoilSpring.Adjust(new Vector3(0.02f, 0.02f, 0.02f), new Vector3(0.3f, 0.3f, 0.3f));
        m_RotationRecoilSpring = new Spring(Spring.Type.AddToLocalRotation, m_Camera.transform);
        m_RotationRecoilSpring.Adjust(new Vector3(0.02f, 0.02f, 0.02f), new Vector3(0.3f, 0.3f, 0.3f));
        m_PositionRecoilSpring.LerpSpeed = m_Springs.SpringLerpSpeed;
        m_RotationRecoilSpring.LerpSpeed = m_Springs.SpringLerpSpeed;
    }

    public void ApplyRecoil(RecoilForce force, float forceMultiplier = 1f){
        force.PlayRecoilForce(forceMultiplier, m_RotationRecoilSpring, m_PositionRecoilSpring);
    }
    public void DoShake(CameraShakeSettings shake, float scale){
		m_Shakes.Add(new CameraShake(shake, m_PositionShakeSpring, m_RotationShakeSpring, scale));
	}
    public void AddExplosionShake(float scale){
        m_Shakes.Add(new CameraShake(m_CamShakes.ExplosionShake, m_PositionShakeSpring, m_RotationShakeSpring, scale));
    }

    private void FixedUpdate(){
        m_PositionSpring_Force.FixedUpdate();
		m_RotationSpring_Force.FixedUpdate();

        m_PositionShakeSpring.FixedUpdate();
        m_RotationShakeSpring.FixedUpdate();

        m_PositionRecoilSpring.FixedUpdate();
        m_RotationRecoilSpring.FixedUpdate();

        UpdateShakes();
    }
    private void Update(){
        m_PositionSpring_Force.Update();
		m_RotationSpring_Force.Update();

        m_PositionShakeSpring.Update();
        m_RotationShakeSpring.Update();

        m_PositionRecoilSpring.Update();
        m_RotationRecoilSpring.Update();

        UpdateFov();
    }

    private void UpdateShakes(){
        if (m_Shakes.Count == 0)
            return;
        int i = 0;
        while (true){
            if (m_Shakes[i].IsDone)
                m_Shakes.RemoveAt(i);
            else{
                m_Shakes[i].Update();
                i++;
            }
            if (i >= m_Shakes.Count)
                break;
        }
    }

    private void UpdateFov(){
        if(m_Camera!=null){
            if(m_Camera.fieldOfView!=targetFov){
                m_Camera.fieldOfView=Mathf.MoveTowards(m_Camera.fieldOfView,targetFov,fovLerpSpeed*Time.deltaTime);
            }
        }
    }
    public void SetViewScale(float viewScale,float lerpSpeed=50.0f){
        fovLerpSpeed=lerpSpeed;
        float origL=(float)Math.Sin(baseFov*Mathf.Deg2Rad/2);
        float targetL=origL/viewScale;
        targetFov=Mathf.Asin(targetL)*Mathf.Rad2Deg*2;
    }
}
