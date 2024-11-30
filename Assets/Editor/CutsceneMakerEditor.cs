using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEngine.Rendering.VolumeComponent;
using Cinemachine.Editor;

[CustomEditor(typeof(CutsceneController))]
public class CutsceneMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CutsceneController controller = (CutsceneController)target;

        var controllerData = serializedObject.FindProperty("cutsceneData");
        for (int i = 0; i < controllerData.arraySize; i++)
        {
            bool skip1 = false;
            bool skip2 = false;
            bool skip3 = false;
            var controllerType = controllerData.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            //GUILayout.ExpandWidth(false);
            controllerType.FindPropertyRelative("isUnfolded").boolValue = EditorGUILayout.Foldout(controllerType.FindPropertyRelative("isUnfolded").boolValue, "Act " + (i + 1));
            //if(!controllerType.FindPropertyRelative("isUnfolded").boolValue)
            
            EditorGUILayout.LabelField(((CutsceneType)controllerType.FindPropertyRelative("cutsceneType").enumValueIndex).ToString(), GUILayout.Width(60));
            EditorGUILayout.LabelField(controllerType.FindPropertyRelative("animationComment").stringValue, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add", "Add Copy Of Action Set"), GUILayout.Width(40)))
            {
                controllerData.InsertArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button(new GUIContent("Remove", "Remove Current Dialogue Set"), GUILayout.Width(60)) && controllerData.arraySize > 0)
            {
                controllerData.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                skip1 = true;
                i--;
            }

            EditorGUILayout.EndHorizontal();
            if (!skip1)
            {
                if (controllerType.FindPropertyRelative("isUnfolded").boolValue)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.PropertyField(controllerType.FindPropertyRelative("cutsceneType"));
                    EditorGUILayout.PropertyField(controllerType.FindPropertyRelative("animationComment"));

                    switch ((CutsceneType)controllerType.FindPropertyRelative("cutsceneType").enumValueIndex)
                    {
                        case CutsceneType.Talking:
                            //EditorGUILayout.PropertyField(controllerType.FindPropertyRelative("cutsceneDialogue"));
                            var dialogueData = controllerType.FindPropertyRelative("cutsceneDialogue");
                            for (int j = 0; j < dialogueData.arraySize; j++)
                            {
                                skip2 = false;
                                var cutsceneDialogueClass = dialogueData.GetArrayElementAtIndex(j);
                                EditorGUILayout.BeginHorizontal();
                                cutsceneDialogueClass.FindPropertyRelative("isUnfolded").boolValue = EditorGUILayout.Foldout(cutsceneDialogueClass.FindPropertyRelative("isUnfolded").boolValue, "Cutscene Dialogue Set " + (j + 1));
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button(new GUIContent("Add", "Add Copy Of Dialogue Set"), GUILayout.Width(40)))
                                {
                                    dialogueData.InsertArrayElementAtIndex(j);
                                    serializedObject.ApplyModifiedProperties();
                                }
                                if (GUILayout.Button(new GUIContent("Remove", "Remove Current Dialogue Set"), GUILayout.Width(60)) && controllerData.arraySize > 0)
                                {
                                    dialogueData.DeleteArrayElementAtIndex(j);
                                    serializedObject.ApplyModifiedProperties();
                                    skip2 = true;
                                    j--;
                                }
                                EditorGUILayout.EndHorizontal();

                                if (!skip2)
                                {
                                    if (cutsceneDialogueClass.FindPropertyRelative("isUnfolded").boolValue)
                                    {
                                        EditorGUILayout.BeginVertical("box");
                                        EditorGUILayout.PropertyField(cutsceneDialogueClass.FindPropertyRelative("lookAtPos"));

                                        EditorGUILayout.PropertyField(cutsceneDialogueClass.FindPropertyRelative("dialogueEmote"));

                                        var dialogueInnerData = cutsceneDialogueClass.FindPropertyRelative("dialogue");
                                        for (int m = 0; m < dialogueInnerData.arraySize; m++)
                                        {
                                            skip3 = false;
                                            var cutsceneDialoguePairing = dialogueInnerData.GetArrayElementAtIndex(m);
                                            EditorGUILayout.BeginHorizontal();
                                            cutsceneDialoguePairing.FindPropertyRelative("isUnfolded").boolValue = EditorGUILayout.Foldout(cutsceneDialoguePairing.FindPropertyRelative("isUnfolded").boolValue, "Cutscene Character/Text Set " + (m + 1));
                                            //if(!controllerType.FindPropertyRelative("isUnfolded").boolValue)
                                            //{
                                            GUILayout.FlexibleSpace();
                                            if (GUILayout.Button(new GUIContent("Add", "Add Copy Of Character Dialogue Set"), GUILayout.Width(40)))
                                            {
                                                dialogueInnerData.InsertArrayElementAtIndex(m);
                                                serializedObject.ApplyModifiedProperties();
                                            }
                                            if (GUILayout.Button(new GUIContent("Remove", "Remove Current Character Dialogue Set"), GUILayout.Width(60)) && dialogueInnerData.arraySize > 0)
                                            {
                                                dialogueInnerData.DeleteArrayElementAtIndex(m);
                                                serializedObject.ApplyModifiedProperties();
                                                skip3 = true;
                                                m--;
                                            }

                                            EditorGUILayout.EndHorizontal();

                                            if (!skip3)
                                            {

                                                if (cutsceneDialoguePairing.FindPropertyRelative("isUnfolded").boolValue)
                                                {
                                                    EditorGUILayout.BeginVertical("box");
                                                    EditorGUILayout.BeginHorizontal();

                                                    EditorGUILayout.LabelField("Character", GUILayout.Width(80));
                                                    EditorGUILayout.PropertyField(cutsceneDialoguePairing.FindPropertyRelative("character"), GUIContent.none);
                                                    EditorGUILayout.LabelField("Ending Mark", GUILayout.Width(80));
                                                    EditorGUILayout.PropertyField(cutsceneDialoguePairing.FindPropertyRelative("ender"), GUIContent.none);
                                                    EditorGUILayout.EndHorizontal();


                                                    EditorGUILayout.BeginHorizontal();
                                                    EditorGUILayout.PropertyField(cutsceneDialoguePairing.FindPropertyRelative("text"));
                                                    EditorGUILayout.EndHorizontal();
                                                    EditorGUILayout.EndVertical();
                                                }
                                            }

                                        }

                                        EditorGUILayout.EndVertical();
                                    }
                                }
                            }
                            break;
                        case CutsceneType.Walking:
                            //EditorGUILayout.PropertyField(controllerType.FindPropertyRelative("cutsceneLocations"));
                            var locationData = controllerType.FindPropertyRelative("cutsceneLocations");

                            for (int j = 0; j < locationData.arraySize; j++)
                            {
                                skip2 = false;
                                var cutsceneLocationClass = locationData.GetArrayElementAtIndex(j);
                                EditorGUILayout.BeginHorizontal();
                                cutsceneLocationClass.FindPropertyRelative("isUnfolded").boolValue = EditorGUILayout.Foldout(cutsceneLocationClass.FindPropertyRelative("isUnfolded").boolValue, "Cutscene Movement Set " + (j + 1));
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button(new GUIContent("Add", "Add Copy Of Movement Locations Set"), GUILayout.Width(40)))
                                {
                                    locationData.InsertArrayElementAtIndex(j);
                                    serializedObject.ApplyModifiedProperties();
                                }
                                if (GUILayout.Button(new GUIContent("Remove", "Remove Current Movement Locations Set"), GUILayout.Width(60)) && locationData.arraySize > 0)
                                {
                                    locationData.DeleteArrayElementAtIndex(j);
                                    serializedObject.ApplyModifiedProperties();
                                    skip2 = true;
                                    j--;
                                }
                                EditorGUILayout.EndHorizontal();

                                if (!skip2)
                                {
                                    if (cutsceneLocationClass.FindPropertyRelative("isUnfolded").boolValue)
                                    {
                                        EditorGUILayout.BeginVertical("box");

                                        var locationsInnerData = cutsceneLocationClass.FindPropertyRelative("locations");
                                        for (int m = 0; m < locationsInnerData.arraySize; m++)
                                        {
                                            skip3 = false;
                                            var cutsceneLocationPairing = locationsInnerData.GetArrayElementAtIndex(m);
                                            EditorGUILayout.BeginHorizontal();
                                            cutsceneLocationPairing.FindPropertyRelative("isUnfolded").boolValue = EditorGUILayout.Foldout(cutsceneLocationPairing.FindPropertyRelative("isUnfolded").boolValue, "Cutscene Character/Location Set " + (m + 1));
                                            //if(!controllerType.FindPropertyRelative("isUnfolded").boolValue)
                                            //{
                                            GUILayout.FlexibleSpace();
                                            if (GUILayout.Button(new GUIContent("Add", "Add Copy Of Movement Character Location Set"), GUILayout.Width(40)))
                                            {
                                                locationsInnerData.InsertArrayElementAtIndex(m);
                                                serializedObject.ApplyModifiedProperties();
                                            }
                                            if (GUILayout.Button(new GUIContent("Remove", "Remove Current Movement Locations Set"), GUILayout.Width(60)) && locationsInnerData.arraySize > 0)
                                            {
                                                locationsInnerData.DeleteArrayElementAtIndex(m);
                                                serializedObject.ApplyModifiedProperties();
                                                skip3 = true;
                                                m--;
                                            }

                                            EditorGUILayout.EndHorizontal();

                                            if (!skip3)
                                            {

                                                if (cutsceneLocationPairing.FindPropertyRelative("isUnfolded").boolValue)
                                                {
                                                    EditorGUILayout.BeginVertical("box");
                                                    EditorGUILayout.BeginHorizontal();

                                                    EditorGUILayout.LabelField("Character", GUILayout.Width(80));
                                                    EditorGUILayout.PropertyField(cutsceneLocationPairing.FindPropertyRelative("character"), GUIContent.none);
                                                    EditorGUILayout.LabelField("Walking Speed", GUILayout.Width(90));
                                                    EditorGUILayout.PropertyField(cutsceneLocationPairing.FindPropertyRelative("speedToLocation"), GUIContent.none);
                                                    EditorGUILayout.EndHorizontal();
                                                    EditorGUILayout.PropertyField(cutsceneLocationPairing.FindPropertyRelative("location"));
                                                    EditorGUILayout.EndVertical();
                                                }
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                }
                            }
                            break;
                        case CutsceneType.Both:

                            break;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("cutsceneData").FindPropertyRelative("cutsceneType"));

        //controllerData.GetArrayElementAtIndex(i).FindPropertyRelative("cutsceneLocations").GetArrayElementAtIndex(j).FindPropertyRelative("locations").GetArrayElementAtIndex(m).FindPropertyRelative("location")

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        CutsceneController controller = (CutsceneController)target;
        var controllerData = serializedObject.FindProperty("cutsceneData");
        for (int i = 0; i < controllerData.arraySize; i++)
        {
            var controllerType = controllerData.GetArrayElementAtIndex(i);

            switch ((CutsceneType)controllerType.FindPropertyRelative("cutsceneType").enumValueIndex)
            {
                case CutsceneType.Talking:
                    GUIStyle talkingStyle = new GUIStyle();
                    talkingStyle.normal.textColor = Color.green;
                    talkingStyle.normal.background = Texture2D.grayTexture;
                    var dialogueData = controllerType.FindPropertyRelative("cutsceneDialogue");
                    Vector3[] newFaceDirection = new Vector3[dialogueData.arraySize];
                    for (int j = 0; j < dialogueData.arraySize; j++)
                    {
                        var cutsceneDialogueClass = dialogueData.GetArrayElementAtIndex(j);

                        //if(j >0)
                        //{
                        //    Handles.DrawAAPolyLine(5f, dialogueData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("lookAtPos").vector3Value, dialogueData.GetArrayElementAtIndex(j).FindPropertyRelative("lookAtPos").vector3Value);
                        //}

                        if (cutsceneDialogueClass.FindPropertyRelative("isUnfolded").boolValue && controllerType.FindPropertyRelative("isUnfolded").boolValue)
                        {
                            if (j == 0)
                            {
                                if (i > 0 && (CutsceneType)controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneType").enumValueIndex == CutsceneType.Talking)
                                {
                                    Handles.color = Color.yellow;
                                    Handles.DrawAAPolyLine(5f, controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneDialogue").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneDialogue").arraySize - 1).FindPropertyRelative("lookAtPos").vector3Value, dialogueData.GetArrayElementAtIndex(j).FindPropertyRelative("lookAtPos").vector3Value);
                                }
                                else if (i > 0 && (CutsceneType)controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneType").enumValueIndex == CutsceneType.Walking)
                                {
                                    Handles.color = Color.red;
                                    Handles.DrawAAPolyLine(5f, controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").arraySize - 1).FindPropertyRelative("locations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").arraySize - 1).FindPropertyRelative("locations").arraySize - 1).FindPropertyRelative("location").vector3Value, dialogueData.GetArrayElementAtIndex(j).FindPropertyRelative("lookAtPos").vector3Value);
                                }
                            }
                            else if (j > 0)
                            {
                                Handles.color = Color.green;
                                Handles.DrawAAPolyLine(5f, dialogueData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("lookAtPos").vector3Value, dialogueData.GetArrayElementAtIndex(j).FindPropertyRelative("lookAtPos").vector3Value);
                            }

                            EditorGUI.BeginChangeCheck();
                            newFaceDirection[j] = Handles.PositionHandle(cutsceneDialogueClass.FindPropertyRelative("lookAtPos").vector3Value, Quaternion.identity);

                            Handles.Label(newFaceDirection[j], "Act " + (i + 1) + " Dialogue Facing Direction " + (j + 1), talkingStyle);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(controller, "Change Look At Position");
                                cutsceneDialogueClass.FindPropertyRelative("lookAtPos").vector3Value = newFaceDirection[j];
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    break;
                case CutsceneType.Walking:
                    GUIStyle walkingStyle = new GUIStyle();
                    walkingStyle.normal.textColor = Color.blue;
                    walkingStyle.normal.background = Texture2D.grayTexture;
                    var locationData = controllerType.FindPropertyRelative("cutsceneLocations");

                    for (int j = 0; j < locationData.arraySize; j++)
                    {
                        var cutsceneLocationClass = locationData.GetArrayElementAtIndex(j);

                        if (cutsceneLocationClass.FindPropertyRelative("isUnfolded").boolValue)
                        {
                            var locationsInnerData = cutsceneLocationClass.FindPropertyRelative("locations");
                            Vector3[] newWalkingDestination = new Vector3[locationsInnerData.arraySize];
                            for (int m = 0; m < locationsInnerData.arraySize; m++)
                            {
                                var cutsceneLocationPairing = locationsInnerData.GetArrayElementAtIndex(m);
                                if (cutsceneLocationPairing.FindPropertyRelative("isUnfolded").boolValue && cutsceneLocationClass.FindPropertyRelative("isUnfolded").boolValue && controllerType.FindPropertyRelative("isUnfolded").boolValue)
                                {
                                    if (m == 0)
                                    {
                                        if (j > 0)
                                        {
                                            Handles.color = Color.yellow;
                                            Handles.DrawAAPolyLine(5f, locationData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("locations").GetArrayElementAtIndex(locationData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("locations").arraySize - 1).FindPropertyRelative("location").vector3Value, locationsInnerData.GetArrayElementAtIndex(m).FindPropertyRelative("location").vector3Value);
                                        }
                                        else if (j == 0 && i > 0 && (CutsceneType)controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneType").enumValueIndex == CutsceneType.Talking)
                                        {
                                            Handles.color = Color.red;
                                            Handles.DrawAAPolyLine(5f, controllerData.GetArrayElementAtIndex(i-1).FindPropertyRelative("cutsceneDialogue").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneDialogue").arraySize - 1).FindPropertyRelative("lookAtPos").vector3Value, locationsInnerData.GetArrayElementAtIndex(m).FindPropertyRelative("location").vector3Value);
                                            //Handles.DrawAAPolyLine(5f, locationData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("locations").GetArrayElementAtIndex(locationData.GetArrayElementAtIndex(j - 1).FindPropertyRelative("locations").arraySize - 1).FindPropertyRelative("location").vector3Value, locationsInnerData.GetArrayElementAtIndex(m).FindPropertyRelative("location").vector3Value);
                                        }
                                        else if (j == 0 && i > 0 && (CutsceneType)controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneType").enumValueIndex == CutsceneType.Walking)
                                        {
                                            Handles.color = Color.yellow;
                                            Handles.DrawAAPolyLine(5f, controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").arraySize - 1).FindPropertyRelative("locations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").GetArrayElementAtIndex(controllerData.GetArrayElementAtIndex(i - 1).FindPropertyRelative("cutsceneLocations").arraySize - 1).FindPropertyRelative("locations").arraySize - 1).FindPropertyRelative("location").vector3Value, locationsInnerData.GetArrayElementAtIndex(m).FindPropertyRelative("location").vector3Value);
                                        }
                                    }
                                    else if (m > 0)
                                    {
                                        Handles.color = Color.green;
                                        Handles.DrawAAPolyLine(5f, locationsInnerData.GetArrayElementAtIndex(m - 1).FindPropertyRelative("location").vector3Value, locationsInnerData.GetArrayElementAtIndex(m).FindPropertyRelative("location").vector3Value);
                                    }

                                    EditorGUILayout.PropertyField(cutsceneLocationPairing.FindPropertyRelative("location"));
                                    EditorGUI.BeginChangeCheck();
                                    newWalkingDestination[m] = Handles.PositionHandle(cutsceneLocationPairing.FindPropertyRelative("location").vector3Value, Quaternion.identity);

                                    Handles.Label(newWalkingDestination[m], "Act " + (i + 1) + " Walking Set " + (j + 1) + " Destination " + (m + 1), walkingStyle);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(controller, "Change Walking Position");
                                        cutsceneLocationPairing.FindPropertyRelative("location").vector3Value = newWalkingDestination[m];
                                        serializedObject.ApplyModifiedProperties();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case CutsceneType.Both:

                    break;
            }
        }
    }
}