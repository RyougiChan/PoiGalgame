using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public abstract class GalgameScriptTagProperty
    {
        public string tag;
        public string name;
        public string value;
        public float width;
        public float height;
        public float top;
        public float left;
        public Layer layer;
        public string method;
        public bool canskip;
        public bool visible;
    }
}
