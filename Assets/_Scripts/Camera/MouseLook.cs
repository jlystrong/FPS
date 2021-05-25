using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : PlayerComponent
{
    public float SensitivityFactor { get; set; }
    public Vector2 LookAngles
    {
        get => lookAngles;
        set
        {
            lookAngles = value;
        }
    }

    public Vector2 LastMovement { get; private set; }

    [BHeader("General", true)]

    public Transform lookRootTrans = null;
    public Transform playerRootTrans = null;
    public bool invert = false;

    [BHeader("Motion")]
    public float sensitivity = 0.1f;
    public float aimSensitivity = 0.05f;
    public float rollAngle = 10f;
    public float rollSpeed = 3f;

    [BHeader("Rotation Limits")]
    public Vector2 defaultLookLimits=new Vector2(-60f,90f);


    private Vector2 lookAngles;
    private float currentRollAngle;

    private void Awake(){
        SensitivityFactor = 1f;
    }

    public void MoveCamera(float verticalMove, float horizontalMove){
        LookAngles += new Vector2(verticalMove, horizontalMove);
    }

    private void LateUpdate(){
        Vector2 prevLookAngles = lookAngles;
        if (Player.ViewLocked.Is(false) && Player.Health.Get() > 0f){
            LookAround();
        }
        LastMovement = lookAngles - prevLookAngles;
    }

    private void LookAround(){
        float tmpSensitivity=Player.Aim.Active?aimSensitivity:sensitivity;
        tmpSensitivity*=SensitivityFactor;

        lookAngles.x += Player.LookInput.Get().y * tmpSensitivity * (invert ? 1f : -1f);
		lookAngles.y += Player.LookInput.Get().x * tmpSensitivity;

        lookAngles.x = ClampAngle(lookAngles.x, defaultLookLimits.x, defaultLookLimits.y);

		currentRollAngle = Mathf.Lerp(currentRollAngle, Player.LookInput.Get().x * rollAngle, Time.deltaTime * rollSpeed);
		lookRootTrans.localRotation = Quaternion.Euler(lookAngles.x, 0f, 0f);
		playerRootTrans.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);
    }


    private float ClampAngle(float angle, float min, float max) {
		if(angle > 360f)
			angle -= 360f;
		else if(angle < -360f)
			angle += 360f;
		return Mathf.Clamp(angle, min, max);
	}

}
