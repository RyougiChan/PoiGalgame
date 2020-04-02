using Assets.Script.Model.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Model.Components
{
    /// <summary>
    /// A user selector component | 提供玩家可选择选项的选择器
    /// </summary>
    [Serializable]
    public class PSelector : PComponent
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
