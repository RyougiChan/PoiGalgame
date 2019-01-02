#if UNITY_EDITOR
using Assets.Scripts.Models;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editors
{
    public class GalgameObject : ScriptableObject
    {
        /// <summary>
        /// Create A Script
        /// </summary>
        [MenuItem("GameObject/GalgameObject/Script"), MenuItem("Assets/GalgameObject/Script")]
        static void CreateGalgameScript()
        {
            GalgameScript ggs = CreateInstance<GalgameScript>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetFullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/NewGalgameScript.gs.asset");
            AssetDatabase.CreateAsset(ggs, assetFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = ggs;
        }

        /// <summary>
        /// Create A Role
        /// </summary>
        [MenuItem("GameObject/GalgameObject/Role"), MenuItem("Assets/GalgameObject/Role")]
        static void CreateGalgameRole()
        {
            Debug.Log("CreateGalgameScript_Role");
        }
    }
}
#endif