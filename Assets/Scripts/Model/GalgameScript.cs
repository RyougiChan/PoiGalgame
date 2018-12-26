using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class GalgameScript : ScriptableObject
    {
        public string ScriptName;
        public string ScriptName2;
        public List<GalgameScriptProps> ScriptProperties = new List<GalgameScriptProps>();
    }
}
