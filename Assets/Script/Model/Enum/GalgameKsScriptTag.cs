using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public enum GalgameKsScriptTag
    {
        GlobalPropertyTags = 000,
        // Global property tags
        pos = 001,
        style = 002,
        tran = 003,
        effect = 004,
        BasicTags = 100,
        // Basic tags
        chs = 101,
        che = 102,
        br = 103,
        p = 104,
        cl = 105,
        ElementTags = 200,
        // Element tags
        text = 201,
        line = 202,
        bg = 203,
        fg = 204,
        bgm = 205,
        video = 206
    }
}
