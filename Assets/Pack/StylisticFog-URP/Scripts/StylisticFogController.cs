using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [ExecuteInEditMode]
    public class StylisticFogController : MonoBehaviour
    {
        public ForwardRendererData rendererData;
        public bool fogEnabled;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public HeightFogSettings HeightFog = HeightFogSettings.defaultSettings;
        public DistanceFogSettings DistanceFog = DistanceFogSettings.defaultSettings;

        private void OnEnable(){
            ResetFogProperties();
        }

        private void ResetFogProperties(){
            if(rendererData==null){
                return ;
            }
            StylisticFog fogFeature=null;
            for(int i=0;i<rendererData.rendererFeatures.Count;i++){
                if(rendererData.rendererFeatures[i].name.Equals("Fog")){
                    fogFeature=rendererData.rendererFeatures[i] as StylisticFog;
                }
            }
            if(fogFeature==null){
                return ;
            }
            fogFeature.SetActive(fogEnabled);
            fogFeature.settings.renderPassEvent=renderPassEvent;
            fogFeature.settings.DistanceFog=DistanceFog;
            fogFeature.settings.HeightFog=HeightFog;
            rendererData.SetDirty();
        }

        #if UNITY_EDITOR
        void Update(){
            ResetFogProperties();
        }
        #endif
    }
}
