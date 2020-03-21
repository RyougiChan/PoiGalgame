using Assets.Script.Model.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Model.Components
{
    [Serializable]
    public class PSelector
    {
        public SelectorType Type;
        public LinkedList<PText> XTexts; 
        public List<PText> Texts;
        public List<Sprite> Bgs;
        public List<AudioClip> Bgms;
        public bool IsSelected;
        public int SelectedItem;
        public List<PSelectorOption> Options;
    }
}
