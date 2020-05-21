using Assets.Script.Model.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model.Components
{
    /// <summary>
    /// Event/GameValues Judge | 事件/数值裁决组件
    /// </summary>
    [Serializable]
    public class PJudge : PComponent
    {
        /// <summary>
        /// 满足裁决条件的可能数值
        /// </summary>
        public List<GameValues> MeetGameValues;

        /// <summary>
        /// 满足裁决条件时应当触发的一系列事件
        /// </summary>
        public List<PEventItem> Events;

        /// <summary>
        /// 满足裁决条件执行最后跳转的下一个Action的ID
        /// </summary>
        public string NextActionId;
    }
}
