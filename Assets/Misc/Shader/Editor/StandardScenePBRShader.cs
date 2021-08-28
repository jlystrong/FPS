using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;
using UnityEditor.Rendering.Universal;

namespace UnityEditor
{
    public class StandardScenePBRShader : ShaderGUI
    {
        private bool reflectEnv=false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            Material targetMat = materialEditor.target as Material;

            targetMat.shaderKeywords=null;
            if(targetMat.HasProperty("_BumpMap")){
                CoreUtils.SetKeyword(targetMat, "_NORMALMAP", targetMat.GetTexture("_BumpMap"));
            }
            if(targetMat.GetFloat("_Reflect_Env")>0.5){
                CoreUtils.SetKeyword(targetMat, "_ENV_REFLECT", true);
            }else{
                CoreUtils.SetKeyword(targetMat, "_ENV_REFLECT", false);
            }
        }
    }
}