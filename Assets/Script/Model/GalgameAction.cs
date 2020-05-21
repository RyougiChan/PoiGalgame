using Assets.Script.Model.Components;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Script.Model
{
    /// <summary>
    /// A scene in a script | 剧本中的某一幕
    /// </summary>
    [Serializable]
    public class GalgameAction : GalgamePlainAction
    {
        public PSelector Selector; 
        public PGameValuesAdjuster GameValuesAdjuster;
        public PJudge Judge;
        public PBattle Battle;
        public PEvents Events;
    }
}
