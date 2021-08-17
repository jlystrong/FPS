using System;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    public enum FogTypePass
    {
        DistanceOnly = 0,
        HeightOnly = 1,
        Both = 2,
        None
    }
    public enum FogDistanceType
    {
        LINEAR = 0,
        EXP =1,
        EXP2 =2,
    }

    [Serializable]
    public struct DistanceFogSettings
    {
        public bool enabled;
        public bool fogSkybox;
        public Color color;
        public FogDistanceType distanceType;
        public float startDistance;
        public float endDistance;
        
        public static DistanceFogSettings defaultSettings
        {
            get
            {
                return new DistanceFogSettings()
                {
                    enabled = false,
                    fogSkybox = false,
                    color = Color.white,
                    distanceType = FogDistanceType.LINEAR,
                    startDistance = 0f,
                    endDistance = 100f,
                };
            }
        }
    }

    [Serializable]
    public struct HeightFogSettings
    {
        public bool enabled;
        public bool fogSkybox;
        public Color color;
        public float baseHeight;
        public float baseDensity;
        [Tooltip("The rate at which the thickness of the fog decays with altitude.")]
        [Range(0.001f, 1f)]
        public float densityFalloff;

        public static HeightFogSettings defaultSettings
        {
            get
            {
                return new HeightFogSettings()
                {
                    enabled = true,
                    fogSkybox = true,
                    color = Color.white,
                    baseHeight = 0f,
                    baseDensity = 0.1f,
                    densityFalloff = 0.5f,
                };
            }
        }
    }
}
