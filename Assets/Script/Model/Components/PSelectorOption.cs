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
        public List<GalgameAction> Actions;
    }
}
