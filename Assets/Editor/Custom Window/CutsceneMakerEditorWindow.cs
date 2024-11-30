using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CutsceneMakerEditorWindow : ExtendedEditorWindow
{
    /*

    #region
    SerializedProperty cutsceneData;
    SerializedProperty dialogue;
    SerializedProperty lookAtPos;
    SerializedProperty dialogueEmote;
    SerializedProperty locations;
    SerializedProperty isTalking;
    static CutsceneFullController cutsceneController;
    #endregion

    private void OnEnable()
    {
        //cutsceneData = serializedObject.FindProperty("cutsceneData");
        if (isTalking.boolValue)
        {
            dialogue = serializedObject.FindProperty("dialogue");
            dialogueEmote = serializedObject.FindProperty("dialogueEmote");
            lookAtPos = serializedObject.FindProperty("lookAtPos");
        }
        else
        {
            locations = serializedObject.FindProperty("locations");
        }
    }
    public static void Open(CutsceneFullController cutsceneMaker)
    {
        cutsceneController = cutsceneMaker;
        CutsceneMakerEditorWindow window = GetWindow<CutsceneMakerEditorWindow>("Cutscene Maker");
        window.serializedObject = new SerializedObject(cutsceneMaker);
    }

    private void OnGUI()
    {
        serializedObject.Update();

        currentProperty = serializedObject.FindProperty("cutsceneData");
        DrawProperties(currentProperty, true);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(100), GUILayout.ExpandHeight(true));

        DrawSidebar(currentProperty);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        if (selectedProperty != null)
        {
            DrawSelectedPropertiesPanel();
        }
        else
        {
            EditorGUILayout.LabelField("Select an action moment");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        Apply();
    }

    private void DrawSelectedPropertiesPanel()
    {
        currentProperty = selectedProperty;

        EditorGUILayout.LabelField("Test");
        EditorGUILayout.BeginHorizontal("box");
        DrawField("isTalking", true);
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal("box");

        ////DrawField("slot", true);
        //EditorGUILayout.EndHorizontal();
        //DrawField("", true);
        /*
         * can add bool for button press/ type of object
         * if(GUILayout.Button("Talking", EditorStyles.toolbarButton))
         * {
         *      var1 = true;
         *      var2 - false;
         * }
         * the DrawField for what to be shown in var1
         * 
         * EditorGUILayout.BeginVertical("box");
         * DrawField("Something", true);
         * EditorGUILayout.EndVertical();
         * 
         * 
         * 
         * 
         * 
         * }
         */
}
