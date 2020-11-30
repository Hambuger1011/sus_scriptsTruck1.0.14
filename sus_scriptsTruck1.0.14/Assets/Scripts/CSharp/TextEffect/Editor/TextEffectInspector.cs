using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TextEffect))]
public class TextEffectInspector : Editor
{
    private TextEffect comp;

    private SerializedProperty m_GradientType;
    private SerializedProperty m_TopColor;
    private SerializedProperty m_OpenShaderOutLine;
    private SerializedProperty m_MiddleColor;
    private SerializedProperty m_BottomColor;
    private SerializedProperty m_ColorOffset;
    private SerializedProperty m_EnableOutLine;
    private SerializedProperty m_OutLineColor;
    private SerializedProperty m_OutLineWidth;
    private SerializedProperty m_Camera;
    private SerializedProperty m_LerpValue;
    private SerializedProperty m_Alpha;

    private float _alpha;
    private Color _OutColor;
    void OnEnable()
    {
        this.comp = (TextEffect)this.target;

        this.m_GradientType         = this.serializedObject.FindProperty("m_GradientType");
        this.m_TopColor             = this.serializedObject.FindProperty("m_TopColor");
        this.m_OpenShaderOutLine    = this.serializedObject.FindProperty("m_OpenShaderOutLine");
        this.m_MiddleColor          = this.serializedObject.FindProperty("m_MiddleColor");
        this.m_BottomColor          = this.serializedObject.FindProperty("m_BottomColor");
        this.m_ColorOffset          = this.serializedObject.FindProperty("m_ColorOffset");
        this.m_EnableOutLine        = this.serializedObject.FindProperty("m_EnableOutLine");
        this.m_OutLineColor         = this.serializedObject.FindProperty("m_OutLineColor"); 
        this.m_OutLineWidth         = this.serializedObject.FindProperty("m_OutLineWidth");
        this.m_Camera               = this.serializedObject.FindProperty("m_Camera");
        this.m_LerpValue            = this.serializedObject.FindProperty("m_LerpValue");
        this.m_Alpha                = this.serializedObject.FindProperty("m_Alpha");

        this._alpha = this.m_Alpha.floatValue;
        this._OutColor = this.m_OutLineColor.colorValue;
    }


    public override void OnInspectorGUI()
    {

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Graphic", this.comp.TextGraphic, typeof(Text), false);
        GUI.enabled = true;
        this._alpha = EditorGUILayout.Slider("Alpha", this._alpha, 0, 1);
        this.comp.SetAlpah(this._alpha);
        EditorGUILayout.PropertyField(this.m_GradientType);
        EditorGUILayout.PropertyField(this.m_Camera);
        EditorGUILayout.PropertyField(this.m_EnableOutLine);
        if (this.m_EnableOutLine.boolValue)
        {
            EditorGUILayout.PropertyField(this.m_OutLineWidth);
            EditorGUILayout.PropertyField(this.m_LerpValue);

            bool tmp_open_state = EditorGUILayout.Toggle("Open Shader OutLine", this.m_OpenShaderOutLine.boolValue);
            if (tmp_open_state != this.m_OpenShaderOutLine.boolValue)
            {
                this.comp.SetShaderOutLine(tmp_open_state);
            }
        }
            
        EditorGUILayout.PropertyField(this.m_TopColor);
        if (this.m_GradientType.enumValueIndex == 2)
        {
            EditorGUILayout.PropertyField(this.m_MiddleColor);
        }
        if(this.m_GradientType.enumValueIndex != 0)
            EditorGUILayout.PropertyField(this.m_BottomColor);
        
        if(this.m_EnableOutLine.boolValue)
        {
            this._OutColor = EditorGUILayout.ColorField("Out Line Color", this._OutColor);
            this.comp.SetOutLineColor(this._OutColor);
        }
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(this.m_ColorOffset);

        this.comp.UpdateOutLineInfos();
        this.serializedObject.ApplyModifiedProperties();


    }
}
