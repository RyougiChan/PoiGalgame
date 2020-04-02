using Assets.Script.Model.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model.Components
{
    /// <summary>
    /// Game values adjuster : 游戏数值调整器
    /// </summary>
    [Serializable]
    public class PGameValuesAdjuster : PComponent
    {
        /// <summary>
        /// Game values change | 游戏数值变动
        /// </summary>
        public GameValues DeltaGameValues;
    }
}
