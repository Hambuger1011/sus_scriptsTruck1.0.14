using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(CircleImage))]
public class CircleImageEditor : ImageEditor
{
    SerializedProperty fillPercent;

    SerializedProperty isSolidCircle;

    SerializedProperty thickness;

    SerializedProperty segements;

    SerializedProperty uiScale;

    SerializedProperty innerVertices;

    SerializedProperty outterVertices;


    protected override void OnEnable()
    {
        base.OnEnable();

        fillPercent = serializedObject.FindProperty("fillPercent");
        isSolidCircle = serializedObject.FindProperty("isSolidCircle");
        thickness = serializedObject.FindProperty("thickness");
        segements = serializedObject.FindProperty("segements");
        uiScale = serializedObject.FindProperty("uiScale");
        innerVertices = serializedObject.FindProperty("innerVertices");
        outterVertices = serializedObject.FindProperty("outterVertices");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        serializedObject.Update();

        EditorGUILayout.EndFadeGroup();
        //UnityEditor.EditorGUI.PropertyField()
        UnityEditor.EditorGUILayout.PropertyField(fillPercent);
        UnityEditor.EditorGUILayout.PropertyField(isSolidCircle);
        if(!isSolidCircle.boolValue)
        {
            UnityEditor.EditorGUILayout.PropertyField(thickness);
        }
        UnityEditor.EditorGUILayout.PropertyField(segements);
        UnityEditor.EditorGUILayout.PropertyField(uiScale);
        //UnityEditor.EditorGUILayout.PropertyField(innerVertices);
        //UnityEditor.EditorGUILayout.PropertyField(outterVertices);
        EditorGUILayout.EndFadeGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
