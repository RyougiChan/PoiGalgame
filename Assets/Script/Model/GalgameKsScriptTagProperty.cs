using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Model
{
    public class GalgameKsScriptTagProperty
    {
        // Global tag propertys
        public string tag;
        public string name;
        public string value;
        public float width;
        public float height;
        public float top;
        public float left;
        public bool visible;
        public Layer layer;
        public string method;
        public bool canskip;
        public int time;
        // Text-like tag properties
        public float linespacing;
        public Align align;
        public string fcolor;
        public float fsize;
        public FontStyle fstyle;
        public string ffamily;
        // Image|Video|Audio resource properties
        public string bgsrc;
        public string fgsrc;
        public string videosrc;
        public string bgmsrc;
        // Video|Audio tag properties
        public float volume;
        public bool loop;
        public string action;
        // Line tag properties
        public string actor;
        public string voice;
        public string line;
        public string anim;
        // Selector tag properties
        public string selector_type;      // horizontal | vertical
        public string selector_text;      // Option: [A,B,C,...], optional
        public string selector_bg;        // Option: [A,B,C,...], optional
        public string selector_bgm;       // Option: [A,B,C,...], optional
        // Selector Option properties
        public string option_text;
        public string option_bg;
        public string option_bgm;
    }
}
