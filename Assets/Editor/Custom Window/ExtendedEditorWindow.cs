using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    /*
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;

    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if(p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if(p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;


                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Item"))
                {
                    prop.InsertArrayElementAtIndex(prop.arraySize);
                }
                if (GUILayout.Button("Remove Item") && prop.arraySize > 0)
                {
                    prop.DeleteArrayElementAtIndex(prop.arraySize - 1);
                }
                EditorGUILayout.EndHorizontal();
        
    }

    protected void DrawSidebar(SerializedProperty prop)
    {
        foreach (SerializedProperty p in prop)
        {
            if(GUILayout.Button(p.displayName))
            {
                selectedPropertyPath = p.propertyPath;
            }
        }

        if(!string.IsNullOrEmpty(selectedPropertyPath))
        {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
    }

    protected void DrawField(string propName, bool relative)
    {

        if(relative && currentProperty != null)
        {
            Debug.Log(currentProperty.FindPropertyRelative("lookAtPos"));
            //currentProperty.objectReferenceValue.ToString();
            //SerializedProperty element = currentProperty.GetArrayElementAtIndex(0);
            //EditorGUILayout.PropertyField(element.FindPropertyRelative(propName), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAtPos"));
        }
        else if(serializedObject != null)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
        }
    }

    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }

    */
}
