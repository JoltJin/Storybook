using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    /*
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        CutsceneFullController obj = EditorUtility.InstanceIDToObject(instanceId) as CutsceneFullController;
        if (obj !=null)
        {
            CutsceneMakerEditorWindow.Open(obj);
            return true;
        }
        return false;
    }
    */
}
/*
[CustomEditor(typeof(CutsceneFullController))]
public class CutsceneMakerCustomEditor : Editor
{
    
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Open Cutscene Maker"))
        {
            CutsceneMakerEditorWindow.Open((CutsceneFullController)target);
        }
    }
}
*/