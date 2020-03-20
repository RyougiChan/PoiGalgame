using System;
using UnityEngine;

namespace Assets.Script.Model
{
    [Serializable]
    public class PText
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
