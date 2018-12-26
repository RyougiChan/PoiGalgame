using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editors
{
    [CustomEditor(typeof(GalgameScript))]
    public class GalgameScriptEditor : Editor
    {
        SerializedProperty ScriptName;
        SerializedProperty ScriptName2;
        SerializedProperty ScriptProperties;

        private void OnEnable()
        {
            ScriptName = serializedObject.FindProperty("ScriptName");
            ScriptName2 = serializedObject.FindProperty("ScriptName2");
            ScriptProperties = serializedObject.FindProperty("ScriptProperties");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(ScriptName);
            EditorGUILayout.PropertyField(ScriptName2);

            for (int i = 0; i < ScriptProperties.arraySize; i++)
            {
                GUILayout.BeginVertical("GroupBox");
                var state = ScriptProperties.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(state, true);
                GUILayout.BeginHorizontal();
                GUILayout.Space(200);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Add"))
            {
                var index = ScriptProperties.arraySize;
                ScriptProperties.InsertArrayElementAtIndex(index);
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}
