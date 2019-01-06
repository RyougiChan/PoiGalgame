using Assets.Script.Model;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editors
{
    [CustomEditor(typeof(GalgameScript))]
    public class GalgameScriptEditor : Editor
    {
        SerializedProperty ScriptName;
        SerializedProperty ChapterName;
        SerializedProperty ChapterAbstract;
        SerializedProperty Bg;
        SerializedProperty Bgm;
        SerializedProperty GalgameActions;

        private void OnEnable()
        {
            ScriptName = serializedObject.FindProperty("ScriptName");
            ChapterName = serializedObject.FindProperty("ChapterName");
            ChapterAbstract = serializedObject.FindProperty("ChapterAbstract");
            Bg = serializedObject.FindProperty("Bg");
            Bgm = serializedObject.FindProperty("Bgm");
            GalgameActions = serializedObject.FindProperty("GalgameActions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(ScriptName);
            EditorGUILayout.PropertyField(ChapterName);
            EditorGUILayout.PropertyField(ChapterAbstract);
            EditorGUILayout.PropertyField(Bg);
            EditorGUILayout.PropertyField(Bgm);

            for (int i = 0; i < GalgameActions.arraySize; i++)
            {
                GUILayout.BeginVertical("GroupBox");
                var state = GalgameActions.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(state, true);
                GUILayout.BeginHorizontal();
                GUILayout.Space(200);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Add"))
            {
                var index = GalgameActions.arraySize;
                GalgameActions.InsertArrayElementAtIndex(index);
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}
