using System;
using UnityEngine;

[Serializable]
public class EquipmentMotionState
{
    [BHeader("Spring Settings")]

    public Spring.Data PositionSpring = Spring.Data.Default;

    public Spring.Data RotationSpring = Spring.Data.Default;

    [Space(4f)]

    [BHeader("General", order = 2)]

    public bool HasEntryOffset;

    [ShowIf("HasEntryOffset", true)]
    public float EntryOffsetDuration = 1f;

    [ShowIf("HasEntryOffset", true)]
    public float LerpToDefaultOffestSpeed = 2f;

    [ShowIf("HasEntryOffset", true)]
    public OffsetModule EntryOffset;

    [Space]

    public OffsetModule Offset;

    public BobModule Bob;

    public NoiseModule Noise;

    [Space(4f)]

    [BHeader("Additional Forces", order = 2)]

    public SpringForce EnterForce;

    public SpringForce ExitForce;

    public SpringForce PosEnterForce;

    public SpringForce PosExitForce;


    #region Internal
    [Serializable]
    public class OffsetModule
    {
        public bool Enabled = true;

        [ShowIf("Enabled", true)]
        public Vector3 PositionOffset;

        [ShowIf("Enabled", true)]
        public Vector3 RotationOffset;
    }

    [Serializable]
    public class BobModule
    {
        public bool Enabled = true;

        [ShowIf("Enabled", true)]
        public float BobSpeedMultiplier = 1f;

        [ShowIf("Enabled", true)]
        public Vector3 PositionAmplitude = new Vector3(0.35f, 0.5f, 0f);

        [ShowIf("Enabled", true)]
        public Vector3 RotationAmplitude = new Vector3(0.35f, 0.5f, 0f);
    }

    [Serializable]
    public class NoiseModule
    {
        [Range(0f, 1f)]
        public float MaxJitter = 0f;

        [Range(0.01f, 10f)]
        public float NoiseSpeed = 1f;

        public Vector3 PosNoiseAmplitude = Vector3.zero;

        public Vector3 RotNoiseAmplitude = Vector3.zero;
    }
    #endregion
}
