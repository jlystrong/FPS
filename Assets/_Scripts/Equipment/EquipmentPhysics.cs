using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentPhysics : EquipmentComponent
{
    [Serializable]
    public class SwayModule
    {
        public bool Enabled = true;
        [Space]
        public float Multiplier = 0.01f;
        public float AimMultiplier = 0.002f;
        [Space]
        public Vector3 LookPositionSway;
        public Vector3 LookRotationSway;
        public Vector3 FallSway;
        [Space]
        public Vector3 StrafePositionSway;
        public Vector3 StrafeRotationSway;
    }
    [Group]
    public SwayModule Sway = null;

    [Serializable]
    public class InputModule
    {
        [Range(0f, 20f)]
        public float LookInputMultiplier = 1f;
        public float MaxLookInput = 20f;
    }
    [Space]
    [Group]
    public InputModule Input = null;

    [Space(3f)]
    [BHeader("STATES", true, order = 100)]
    public EquipmentMotionState IdleState = null;
    public EquipmentMotionState WalkState = null;
    public EquipmentMotionState RunState = null;
    public EquipmentMotionState AimState = null;

    // void Awake(){
    //     AimState.Offset.PositionOffset=Spring.CalculateReal(AimState.Offset.PositionOffset,AimState.PositionSpring.Stiffness,AimState.PositionSpring.Damping);
    //     AimState.Offset.RotationOffset=Spring.CalculateReal(AimState.Offset.RotationOffset,AimState.RotationSpring.Stiffness,AimState.RotationSpring.Damping);
    // }


}
