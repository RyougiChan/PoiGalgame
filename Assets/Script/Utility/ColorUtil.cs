using System;
using System.Globalization;
using UnityEngine;

namespace Assets.Script.Utility
{
    public class ColorUtil
    {
        public static Color HexToUnityColor(uint hex)
        {
            return new Color((byte)((hex & 0xff000000) >> 0x18), (byte)((hex & 0xff0000) >> 0x10), (byte)((hex & 0xff00) >> 0x08), (byte)(hex & 0xff));
        }
    }
}
