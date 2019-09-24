using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UICustomInputField))]
[CanEditMultipleObjects]
public class UICustomInputFieldEditor : Editor {

    SerializedProperty _rect;

    private void OnEnable() {
        _rect = serializedObject.FindProperty("_rect");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(_rect);
        base.OnInspectorGUI();
    }
}
