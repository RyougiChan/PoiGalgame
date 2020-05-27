using System;
using UnityEngine;

namespace Assets.Script.Model
{
    [Serializable]
    public class GalgameScriptLine
    {
        public string text;
        public string ffamily;
        public string fcolor;
        public float fsize;
        public float linespacing;
        public Align align;
        public FontStyle fstyle;
        public Actor actor;
    }
}
