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
        [Obsolete]
        public LinkedList<PText> XTexts;
        [Obsolete]
        public List<PText> Texts;
        [Obsolete]
        public List<Sprite> Bgs;
        [Obsolete]
        public List<AudioClip> Bgms;
        public bool IsSelected;
        public int SelectedItem;
        public List<PSelectorOption> Options;
    }
}
