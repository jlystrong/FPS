using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : PlayerComponent
{
    private float m_SensitivityFactor=1f;
    public float SensitivityFactor { 
        get=>m_SensitivityFactor; 
        set{
            m_SensitivityFactor=value;
        } 
    }
    public Vector2 LookAngles{
        get => m_LookAngles;
        set
        {
            m_LookAngles = value;
        }
    }

    // public Vector2 LastMovement { get; private set; }

    [BHeader("General", true)]
    [SerializeField]
    private Transform m_LookRoot = null;
    [SerializeField]
    private Transform m_PlayerRoot = null;
    [SerializeField]
    private bool m_Invert = false;

    [BHeader("Motion")]
    [SerializeField]
    private float m_Sensitivity = 0.1f;
    [SerializeField]
    private float m_SensitivityYMul=0.5f;
    [SerializeField]
    private float m_AimSensitivity = 0.05f;
    [SerializeField]
    private float m_SmoothSensitivity=0.5f;
    [SerializeField]
    private float m_SmoothResistance=3f;

    [BHeader("Rotation Limits")]
    [SerializeField]
    private Vector2 m_DefaultLookLimits=new Vector2(-60f,90f);


    private Vector2 m_LookAngles;
    private Vector2 m_SmoothSpeed=Vector2.zero;

    private void Awake(){
        SensitivityFactor = 1f;
    }
    private void Start(){
        m_LookAngles = new Vector2(transform.localEulerAngles.x, m_PlayerRoot.localEulerAngles.y);
    }

    public void MoveCamera(float verticalMove, float horizontalMove){
        LookAngles += new Vector2(verticalMove, horizontalMove);
    }

    private void LateUpdate(){
        Vector2 prevLookAngles = m_LookAngles;
        if (Player.ViewLocked.Is(false)){
            LookAround();
        }
        // LastMovement = m_LookAngles - prevLookAngles;
    }

    private void LookAround(){
        float sensitivity = Player.Aim.Active ? m_AimSensitivity : m_Sensitivity;
        sensitivity *= SensitivityFactor;
        

        Vector2 moveAngles=Vector2.zero;
        moveAngles.x=Player.LookInput.Get().y * (m_Invert ? 1f : -1f);
        moveAngles.y=Player.LookInput.Get().x * m_SensitivityYMul;
        m_LookAngles+=moveAngles*sensitivity;
        m_SmoothSpeed+=moveAngles*m_SmoothSensitivity*sensitivity;

        m_LookAngles+=m_SmoothSpeed*Time.deltaTime;
        m_SmoothSpeed=Vector2.Lerp(m_SmoothSpeed,Vector2.zero,Time.deltaTime*m_SmoothResistance);

        m_LookAngles.x = ClampAngle(m_LookAngles.x, m_DefaultLookLimits.x, m_DefaultLookLimits.y);

		m_LookRoot.localRotation = Quaternion.Euler(m_LookAngles.x, 0f, 0f);
		m_PlayerRoot.localRotation = Quaternion.Euler(0f, m_LookAngles.y, 0f);

        // Entity.LookDirection.Set(m_LookRoot.forward);
    }

    private float ClampAngle(float angle, float min, float max) {
		if(angle > 360f)
			angle -= 360f;
		else if(angle < -360f)
			angle += 360f;
		return Mathf.Clamp(angle, min, max);
	}

}
