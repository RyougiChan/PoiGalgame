using System;
using UnityEngine;

namespace Assets.Script.Model.Components
{
    [Serializable]
    public class PText : PComponent
    {
        public string text;
        public string ffamily;
        public string fcolor;
        public float fsize;
        public float linespacing;
        public Align align;
        public FontStyle fstyle;
    }
}
