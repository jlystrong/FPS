using System;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [Serializable]
    public class StylisticFogSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public HeightFogSettings HeightFog = HeightFogSettings.defaultSettings;
        public DistanceFogSettings DistanceFog = DistanceFogSettings.defaultSettings;
        public Material Material = null;
    }
}
