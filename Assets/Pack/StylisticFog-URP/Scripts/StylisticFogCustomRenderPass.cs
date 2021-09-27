using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public class StylisticFogCustomRenderPass : ScriptableRenderPass
    {
        private string m_ProfilerTag;
        private RenderTargetIdentifier m_TmpRT1;
        private RenderTargetIdentifier m_Source;

        public Material Material;
        public DistanceFogSettings distanceFog = DistanceFogSettings.defaultSettings;
        public HeightFogSettings heightFog = HeightFogSettings.defaultSettings;

        RenderTextureDescriptor m_Descriptor;
        readonly GraphicsFormat m_DefaultHDRFormat;
        bool m_UseRGBM;

        public void Setup(RenderTargetIdentifier source)
        {
            m_Source = source;
        }

        public StylisticFogCustomRenderPass(string profilerTag)
        {
            m_ProfilerTag = profilerTag;

            // Texture format pre-lookup
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render))
            {
                m_DefaultHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                m_UseRGBM = false;
            }
            else
            {
                m_DefaultHDRFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
                m_UseRGBM = true;
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;
            m_Descriptor=cameraTextureDescriptor;
            m_Descriptor.depthBufferBits = 0;
            m_Descriptor.msaaSamples = 1;
            m_Descriptor.width = width;
            m_Descriptor.height = height;
            m_Descriptor.graphicsFormat = m_DefaultHDRFormat;
            m_Descriptor.useMipMap = false;
            m_Descriptor.autoGenerateMips = false;

            m_TmpRT1 = SetupRenderTargetIdentifier(cmd, 0, width, height);
        }

        private RenderTargetIdentifier SetupRenderTargetIdentifier(CommandBuffer cmd, int id, int width, int height)
        {
            int tmpId = Shader.PropertyToID($"StylisticFog_{id}_RT");
            cmd.GetTemporaryRT(tmpId,m_Descriptor,FilterMode.Bilinear);
            // cmd.GetTemporaryRT(tmpId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            var rt = new RenderTargetIdentifier(tmpId);
            ConfigureTarget(rt);

            return rt;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Material == null)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(m_ProfilerTag);
            var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            var fogType = SetMaterialUniforms(renderingData.cameraData.camera);
            if (fogType == FogTypePass.None)
            {
                Blit(cmd, m_Source, m_Source);
            }
            else
            {
                Blit(cmd, m_Source, m_TmpRT1, Material, (int)fogType);
                Blit(cmd, m_TmpRT1, m_Source);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }

        private FogTypePass SetMaterialUniforms(Camera camera)
        {
            // Determine the fog type pass
            FogTypePass fogType = FogTypePass.DistanceOnly;

            if (!distanceFog.enabled && heightFog.enabled)
                fogType = FogTypePass.HeightOnly;

            if (distanceFog.enabled && heightFog.enabled)
            {
                fogType = FogTypePass.Both;
            }

            if (!distanceFog.enabled && !heightFog.enabled)
                return FogTypePass.None;

            // Get the inverse view matrix for converting depth to world position.
            Matrix4x4 inverseViewMatrix = camera.cameraToWorldMatrix;
            Material.SetMatrix("_InverseViewMatrix", inverseViewMatrix);

            // Decide wheter the skybox should have fog applied
            Material.SetInt("_ApplyDistToSkybox", distanceFog.fogSkybox ? 1 : 0);
            Material.SetInt("_ApplyHeightToSkybox", heightFog.fogSkybox ? 1 : 0);

            // Set distance fog properties
            if (distanceFog.enabled)
            {
                SetDistanceFogUniforms();
            }
            // Set height fog properties
            if (heightFog.enabled)
            {
                SetHeightFogUniforms();
            }

            return fogType;
        }

        private void SetDistanceFogUniforms()
        {
            Material.SetVector("_DistanceColor", distanceFog.color);
            Material.SetFloat("_FogStartDistance", distanceFog.startDistance);
            Material.SetFloat("_FogEndDistance", distanceFog.endDistance);
            Material.SetFloat("_FogDistanceType", (float)distanceFog.distanceType);
        }

        private void SetHeightFogUniforms()
        {
            Material.SetVector("_HeightColor", heightFog.color);
            Material.SetFloat("_Height", heightFog.baseHeight);
            Material.SetFloat("_BaseDensity", heightFog.baseDensity);
            Material.SetFloat("_DensityFalloff", heightFog.densityFalloff);
        }
    }
}
