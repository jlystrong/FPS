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
        public float multiplier = 0.1f;
        public float AimMultiplier = 0.02f;
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

}
