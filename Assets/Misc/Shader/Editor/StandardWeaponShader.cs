using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;
using UnityEditor.Rendering.Universal;

namespace UnityEditor
{
    public class StandardWeaponShader : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            Material targetMat = materialEditor.target as Material;

            targetMat.shaderKeywords=null;
            if(targetMat.HasProperty("_BumpMap")){
                CoreUtils.SetKeyword(targetMat, "_NORMALMAP", targetMat.GetTexture("_BumpMap"));
            }
        }
    }
}