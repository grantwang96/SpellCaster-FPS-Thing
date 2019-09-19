using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UICustomButton))]
[CanEditMultipleObjects]
public class UICustomButtonEditor : Editor {

    SerializedProperty _customButtonText;

    private void OnEnable() {
        _customButtonText = serializedObject.FindProperty("_customButtonText");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_customButtonText);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
