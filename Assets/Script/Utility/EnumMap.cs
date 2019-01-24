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
            { Align.lt, TextAnchor.UpperLeft },
            { Align.lm, TextAnchor.MiddleLeft },
            { Align.lb, TextAnchor.LowerLeft },
            { Align.mt, TextAnchor.UpperCenter },
            { Align.mm, TextAnchor.MiddleCenter },
            { Align.mb, TextAnchor.LowerCenter },
            { Align.rt, TextAnchor.UpperRight },
            { Align.rm, TextAnchor.MiddleRight },
            { Align.rb, TextAnchor.LowerRight }
        };

        public static TextAnchor AlignToTextAnchor(Align align)
        {
            return alignTextAnchorMap[align];
        }
    }
}
