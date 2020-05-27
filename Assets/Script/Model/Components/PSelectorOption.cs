using Assets.Script.Model.Datas;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Model.Components
{
    [Serializable]
    public class PSelectorOption : PComponent
    {
        public PText Text;
        public Sprite Bg;
        public AudioClip Bgm;
        // There should be properties about Game-Value
        public GameValues DeltaGameValues;
        public GalgamePlainAction Action;
    }
}
