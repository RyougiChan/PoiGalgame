using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Model.Components
{
    [Serializable]
    public class PSelectorOption
    {
        public PText Text;
        public Sprite Bg;
        public AudioClip Bgm;
        // There should be properties about Game-Value
        // public List<string> Target;
        // public int Value;
        public List<GalgamePlainAction> Actions;
    }
}
