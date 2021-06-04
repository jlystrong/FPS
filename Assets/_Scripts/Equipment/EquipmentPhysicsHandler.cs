using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EquipmentPhysicsHandler : PlayerComponent
{
    [Serializable]
    private class GeneralSettings
    {
        public float SpringLerpSpeed = 25f;
        public float PositionBobOffset = 0f;
        public float RotationBobOffset = 0.5f;
        public float SpringForceMultiplier = 1f;
    }
    [SerializeField]
    [Group]
    private GeneralSettings m_GeneralSettings = null;
	
    public Spring PositionSpring { get; private set; }
	public Spring RotationSpring { get; private set; }

    private EquipmentHandler m_FPHandler;
    private EquipmentMotionState m_CurrentState;
    private EquipmentPhysics m_EquipmentPhysics;

    private Transform m_Pivot;
    private Transform m_Model;
	private Transform m_Root;

    private Vector3 m_OriginalRootPosition;
	private Quaternion m_OriginalRootRotation;
    private Vector3 m_ModelOriginalPos;

    private Vector3 m_SwayVelocity;
    private Vector2 m_LookInput;

    private float m_ChangeToDefaultOffestTime;
    private float m_LerpedOffset;
    private Vector3 m_StatePosition;
    private Vector3 m_StateRotation;

    // Bob
    private int m_LastFootDown;
    private float m_CurrentBobParam;

    // State visualization
    private EquipmentMotionState m_StateToVisualize = null;
    private float m_VisualizationSpeed = 4f;
    private bool m_FirstStepTriggered;

    void Awake(){
        m_Pivot=transform;
        m_FPHandler = GetComponent<EquipmentHandler>();
        m_FPHandler.OnSelected+=OnChangeItem;

        PositionSpring = new Spring(Spring.Type.OverrideLocalPosition, m_Pivot, Vector3.zero);
		RotationSpring = new Spring(Spring.Type.OverrideLocalRotation, m_Pivot, Vector3.zero);
        RotationSpring.LerpSpeed = m_GeneralSettings.SpringLerpSpeed;
		PositionSpring.LerpSpeed = m_GeneralSettings.SpringLerpSpeed;
    }

    private void OnChangeItem(bool selected){
        if(!selected){
            return ;
        }
        m_Root=m_FPHandler.ItemTransform;
        m_Pivot=GetComponentInChildren<EquipmentPivot>().transform;
        m_EquipmentPhysics = m_FPHandler.ItemTransform.GetComponent<EquipmentPhysics>();

        ResetSpring();
        SetOffset();
    }
    public void ResetSpring(){
        PositionSpring.Reset();
        RotationSpring.Reset();
        m_StatePosition = m_StateRotation = Vector3.zero;
        m_CurrentState = null;
        PositionSpring = new Spring(Spring.Type.OverrideLocalPosition, m_Pivot, Vector3.zero);
		RotationSpring = new Spring(Spring.Type.OverrideLocalRotation, m_Pivot, Vector3.zero);
        RotationSpring.LerpSpeed = m_GeneralSettings.SpringLerpSpeed;
		PositionSpring.LerpSpeed = m_GeneralSettings.SpringLerpSpeed;
    }
    public void SetOffset(){
        m_Root.localPosition = Vector3.zero;
        m_Root.localRotation = Quaternion.identity;
    }

    private void FixedUpdate(){
        if (m_EquipmentPhysics == null){
			return;
        }
        m_LookInput = Player.LookInput.Get();
        m_LookInput *= m_EquipmentPhysics.Input.LookInputMultiplier;
        m_LookInput = Vector2.ClampMagnitude(m_LookInput, m_EquipmentPhysics.Input.MaxLookInput);
        m_StatePosition = Vector3.zero;
		m_StateRotation = Vector3.zero;

        UpdateState();
        UpdateOffset();
        UpdateBob();
		UpdateSway();
		UpdateNoise();

        m_StatePosition *= m_GeneralSettings.SpringForceMultiplier;
		m_StateRotation *= m_GeneralSettings.SpringForceMultiplier;
        PositionSpring.AddForce(m_StatePosition);
		RotationSpring.AddForce(m_StateRotation);
        RotationSpring.FixedUpdate();
        PositionSpring.FixedUpdate();
    }
    private void Update(){
        if (PositionSpring != null && RotationSpring != null){
            RotationSpring.Update();
            PositionSpring.Update();
        }
    }


    private void UpdateState(){
        if (Player.Run.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
            TrySetState(m_EquipmentPhysics.RunState);
        else if (Player.Aim.Active)
            TrySetState(m_EquipmentPhysics.AimState);
        else if (Player.Walk.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
            TrySetState(m_EquipmentPhysics.WalkState);
        else
            TrySetState(m_EquipmentPhysics.IdleState);
    }
    private void TrySetState(EquipmentMotionState state){
        if (m_CurrentState != state){
            if (m_CurrentState != null){
                if ((m_CurrentState.HasEntryOffset && m_ChangeToDefaultOffestTime < Time.time) || !m_CurrentState.HasEntryOffset){
                    if (!(state == m_EquipmentPhysics.AimState)){
                        RotationSpring.AddForce(m_CurrentState.ExitForce);
                        PositionSpring.AddForce(m_CurrentState.PosExitForce);
                    }
                }
            }
            m_CurrentState = state;
            PositionSpring.Adjust(state.PositionSpring);
            RotationSpring.Adjust(state.RotationSpring);
            if (m_CurrentState != null){
                if (m_CurrentState.HasEntryOffset)
                    m_ChangeToDefaultOffestTime = Time.time + m_CurrentState.EntryOffsetDuration;
                m_LerpedOffset = 0f;
                RotationSpring.AddForce(m_CurrentState.EnterForce);
                PositionSpring.AddForce(m_CurrentState.PosEnterForce);
            }
        }
    }
    private void UpdateOffset(){
        if (!m_CurrentState.Offset.Enabled || Player.Reload.Active)
            return;

        if (m_CurrentState.HasEntryOffset){
            if (m_ChangeToDefaultOffestTime > Time.time){
                m_StatePosition += m_CurrentState.EntryOffset.PositionOffset*0.0001f;
                m_StateRotation += m_CurrentState.EntryOffset.RotationOffset*0.001f;
            }
            else{
                m_LerpedOffset = Mathf.Lerp(m_LerpedOffset, 1, Time.deltaTime * m_CurrentState.LerpToDefaultOffestSpeed);
                m_StatePosition += m_CurrentState.Offset.PositionOffset * m_LerpedOffset*0.0001f;
                m_StateRotation += m_CurrentState.Offset.RotationOffset * m_LerpedOffset*0.0001f;
            }
        }
        else{
            m_StatePosition += m_CurrentState.Offset.PositionOffset*0.0001f;
            m_StateRotation += m_CurrentState.Offset.RotationOffset*0.001f;
        }
    }
    private void UpdateBob(){
        if (!m_CurrentState.Bob.Enabled || (Player.Velocity.Get().sqrMagnitude == 0 && m_CurrentState == m_EquipmentPhysics.AimState))
            return;

        m_CurrentBobParam = Player.MoveCycle.Get() * Mathf.PI;
        if (m_LastFootDown != 0)
            m_CurrentBobParam += Mathf.PI;

        // Update position bob
        Vector3 posBobAmplitude = Vector3.zero;

        posBobAmplitude.x = m_CurrentState.Bob.PositionAmplitude.x * -0.00001f;
        posBobAmplitude.y = m_CurrentState.Bob.PositionAmplitude.y * 0.00001f;
        posBobAmplitude.z = m_CurrentState.Bob.PositionAmplitude.z * 0.00001f;

        m_StatePosition.x += Mathf.Cos(m_CurrentBobParam + m_GeneralSettings.PositionBobOffset) * posBobAmplitude.x;
        m_StatePosition.y += Mathf.Cos(m_CurrentBobParam * 2 + m_GeneralSettings.PositionBobOffset) * posBobAmplitude.y;
        m_StatePosition.z += Mathf.Cos(m_CurrentBobParam + m_GeneralSettings.PositionBobOffset) * posBobAmplitude.z;

        // Update rotation bob
        Vector3 rotBobAmplitude = m_CurrentState.Bob.RotationAmplitude * 0.001f;

        m_StateRotation.x += Mathf.Cos(m_CurrentBobParam * 2 + m_GeneralSettings.RotationBobOffset) * rotBobAmplitude.x;
        m_StateRotation.y += Mathf.Cos(m_CurrentBobParam + m_GeneralSettings.RotationBobOffset) * rotBobAmplitude.y;
        m_StateRotation.z += Mathf.Cos(m_CurrentBobParam + m_GeneralSettings.RotationBobOffset) * rotBobAmplitude.z;
    }
    private void UpdateSway(){
        if (!m_EquipmentPhysics.Sway.Enabled){
            return;
        }
		float multiplier = Player.Aim.Active ? m_EquipmentPhysics.Sway.AimMultiplier : m_EquipmentPhysics.Sway.Multiplier;
		multiplier *= Time.fixedDeltaTime;
        m_SwayVelocity = Player.Velocity.Get();
        if (Mathf.Abs(m_SwayVelocity.y) < 1.5f)
			m_SwayVelocity.y = 0f;
        Vector3 localVelocity = transform.InverseTransformDirection(m_SwayVelocity / 60);
        PositionSpring.AddForce(new Vector3(
            m_LookInput.x * m_EquipmentPhysics.Sway.LookPositionSway.x * 0.125f,
            m_LookInput.y * m_EquipmentPhysics.Sway.LookPositionSway.y * -0.125f,
            m_LookInput.y * m_EquipmentPhysics.Sway.LookPositionSway.z * -0.125f) * multiplier);
        RotationSpring.AddForce(new Vector3(
            m_LookInput.y * m_EquipmentPhysics.Sway.LookRotationSway.x * 1.25f,
            m_LookInput.x * m_EquipmentPhysics.Sway.LookRotationSway.y * -1.25f,
            m_LookInput.x * m_EquipmentPhysics.Sway.LookRotationSway.z * -1.25f) * multiplier);

        //Fall
        // var fallSway = m_EquipmentPhysics.Sway.FallSway * m_SwayVelocity.y * 0.2f * multiplier;
        // if (Player.IsGrounded.Get())
        //     fallSway *= (15f * multiplier);
        // fallSway.z = Mathf.Max(0f, m_EquipmentPhysics.Sway.FallSway.z);
        // RotationSpring.AddForce(fallSway);

        // Strafe position sway
        PositionSpring.AddForce(new Vector3(
            localVelocity.x * m_EquipmentPhysics.Sway.StrafePositionSway.x * 0.08f,
            -Mathf.Abs(localVelocity.x * m_EquipmentPhysics.Sway.StrafePositionSway.y * 0.08f),
            -localVelocity.z * m_EquipmentPhysics.Sway.StrafePositionSway.z * 0.08f) * multiplier);
        // Strafe rotation sway
        RotationSpring.AddForce(new Vector3(
            -Mathf.Abs(localVelocity.x * m_EquipmentPhysics.Sway.StrafeRotationSway.x * 8f),
            -localVelocity.x * m_EquipmentPhysics.Sway.StrafeRotationSway.y * 8f,
            localVelocity.x * m_EquipmentPhysics.Sway.StrafeRotationSway.z * 8f) * multiplier);
    }
    private void UpdateNoise(){
        if (m_CurrentState.Noise.PosNoiseAmplitude != Vector3.zero && m_CurrentState.Noise.RotNoiseAmplitude != Vector3.zero){
            float jitter = Random.Range(0, m_CurrentState.Noise.MaxJitter);
            float timeScale = Time.time * m_CurrentState.Noise.NoiseSpeed;

            m_StatePosition.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.x / 1000;
            m_StatePosition.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.y / 1000;
            m_StatePosition.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.z / 1000;

            m_StateRotation.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.x / 10;
            m_StateRotation.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.y / 10;
            m_StateRotation.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.z / 10;
        }
    }
}
