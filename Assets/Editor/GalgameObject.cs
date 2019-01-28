#if UNITY_EDITOR
using Assets.Script.Model;
using Assets.Script.Utility;
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
            GalgameScriptUtil.CreateGalgameScriptAsset();
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