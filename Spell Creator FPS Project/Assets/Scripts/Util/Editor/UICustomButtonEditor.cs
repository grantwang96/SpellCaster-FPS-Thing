﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UICustomButton))]
[CanEditMultipleObjects]
public class UICustomButtonEditor : Editor {

    SerializedProperty _customButtonText;
    SerializedProperty _rect;

    private void OnEnable() {
        _customButtonText = serializedObject.FindProperty("_customButtonText");
        _rect = serializedObject.FindProperty("_rect");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_customButtonText);
        EditorGUILayout.PropertyField(_rect);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
