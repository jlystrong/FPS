﻿using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(WeaponRecoil.CameraRecoilModule))]
public class CameraRecoilDrawer : CopyPasteBase<WeaponRecoil.CameraRecoilModule>
{
    private const string menuName = "Camera Recoil";


    [MenuItem("CONTEXT/" + menuName + "/Copy " + menuName)]
    private static void Copy()
    {
        DoCopy();
    }

    [MenuItem("CONTEXT/" + menuName + "/Paste " + menuName)]
    private static void Paste()
    {
        DoPaste();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OnGUI(position, property, label, menuName);
    }
}
