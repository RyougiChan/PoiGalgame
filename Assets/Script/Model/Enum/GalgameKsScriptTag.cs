using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public enum GalgameKsScriptTag
    {
        GLOBAL_PROPERTY_TAGS = 000,
        // Global property tags
        POS = 001,
        STYLE = 002,
        TRAN = 003,
        EFFECT = 004,
        BASIC_TAGS = 100,
        // Basic tags
        CHS = 101,
        CHE = 102,
        BR = 103,
        P = 104,
        CL = 105,
        GOTO = 106,
        ELEMENT_TAGS = 200,
        // Element tags
        TEXT = 201,
        LINE = 202,
        BG = 203,
        FG = 204,
        BGM = 205,
        VIDEO = 206,
        SELECT = 207,
        OPTION = 208,
        ADJUSTER = 209,
        JUDGE = 210,
        BATTLE = 211,
        GROUP = 212,
        PAIR = 213,
        ACTION = 214,
        EVENTS = 215,
        EVENT = 216
    }
}
