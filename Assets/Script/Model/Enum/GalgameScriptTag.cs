using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public enum GalgameScriptTag
    {
        // Basic tags
        chs,
        che,
        br,
        p,
        cl,
        // Element tags
        text,
        line,
        bg,
        fg,
        font,
        // Action tags
        playbgm,
        pausebgm,
        resumebgm,
        stopbgm,
        playvideo,
        pausevideo,
        resumevideo,
        stopvideo,
        // Global property tags
        pos,
        style,
        tran,
        effect
    }
}
