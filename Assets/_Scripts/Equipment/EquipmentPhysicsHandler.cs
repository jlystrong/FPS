using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentPhysicsHandler : PlayerComponent
{
    [Serializable]
    private class GeneralSettings
    {
        public float SpringPosLerpSpeed = 25f;
        public float SpringRotLerpSpeed = 25f;
        public float PositionBobOffset = 0f;
        public float RotationBobOffset = 0.5f;
        public float SpringForceMultiplier = 1f;
    }
    [SerializeField]
    [Group]
    private GeneralSettings m_GeneralSettings = null;
	public Spring EquipmentSpring { get; private set; }
    private Transform m_Pivot;

    private EquipmentHandler m_FPHandler;
    private EquipmentPhysics m_EquipmentPhysics;


    private Vector3 m_SwayVelocity;
    private Vector2 m_LookInput;
    private Vector3 m_StatePosition;
	private Vector3 m_StateRotation;

    void Awake(){
        m_FPHandler = GetComponent<EquipmentHandler>();
        m_FPHandler.OnSelected+=OnChangeItem;

        m_Pivot=transform.Find("Offset");
    }

    private void OnChangeItem(bool selected){
        if(!selected){
            return ;
        }
        m_EquipmentPhysics = m_FPHandler.ItemTransform.GetComponent<EquipmentPhysics>();
        m_Pivot=m_FPHandler.ItemTransform.Find("Offset1");
        EquipmentSpring=new Spring(m_Pivot);
    }

    public void ResetSpring(){
        m_Pivot.localPosition = Vector3.zero;
		m_Pivot.localRotation = Quaternion.identity;
	}


    private void Update(){
        if (m_EquipmentPhysics == null){
            return;
        }
        m_LookInput = Player.LookInput.Get();
        m_LookInput *= m_EquipmentPhysics.Input.LookInputMultiplier;
		m_LookInput = Vector2.ClampMagnitude(m_LookInput, m_EquipmentPhysics.Input.MaxLookInput);

        UpdateSway();

        EquipmentSpring.Update(m_GeneralSettings.SpringPosLerpSpeed,m_GeneralSettings.SpringRotLerpSpeed);
    }

    private void UpdateSway(){
        if (!m_EquipmentPhysics.Sway.Enabled){
            return;
        }
        float multiplier = Player.Aim.Active ? m_EquipmentPhysics.Sway.AimMultiplier : m_EquipmentPhysics.Sway.multiplier;
        multiplier *= Time.deltaTime;
        m_SwayVelocity = Player.Velocity.Get();
        if (Mathf.Abs(m_SwayVelocity.y) < 1.5f)
        {
            m_SwayVelocity.y = 0f;
        }
        Vector3 localVelocity = transform.InverseTransformDirection(m_SwayVelocity / 60);

        EquipmentSpring.AddRotation(new Vector3(
            m_LookInput.y * m_EquipmentPhysics.Sway.LookRotationSway.x,
            m_LookInput.x * m_EquipmentPhysics.Sway.LookRotationSway.y * -1,
            m_LookInput.x * m_EquipmentPhysics.Sway.LookRotationSway.z * -1) * multiplier);
        EquipmentSpring.AddPosition(new Vector3(
            m_LookInput.x * m_EquipmentPhysics.Sway.LookPositionSway.x,
            m_LookInput.y * m_EquipmentPhysics.Sway.LookPositionSway.y * -1,
            m_LookInput.y * m_EquipmentPhysics.Sway.LookPositionSway.z * -1) * multiplier);
    }
}
