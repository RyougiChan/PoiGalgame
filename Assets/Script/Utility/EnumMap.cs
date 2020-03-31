using Assets.Script.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Utility
{
    public class EnumMap
    {
        static IDictionary<Align, TextAnchor> alignTextAnchorMap = new Dictionary<Align, TextAnchor>()
        {
            { Align.LT, TextAnchor.UpperLeft },
            { Align.LM, TextAnchor.MiddleLeft },
            { Align.LB, TextAnchor.LowerLeft },
            { Align.MT, TextAnchor.UpperCenter },
            { Align.MM, TextAnchor.MiddleCenter },
            { Align.MB, TextAnchor.LowerCenter },
            { Align.RT, TextAnchor.UpperRight },
            { Align.RM, TextAnchor.MiddleRight },
            { Align.RB, TextAnchor.LowerRight }
        };

        public static TextAnchor AlignToTextAnchor(Align align)
        {
            return alignTextAnchorMap[align];
        }
    }
}
