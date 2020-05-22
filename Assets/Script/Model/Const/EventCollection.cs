using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public class EventCollection
    {
        private static EventCollection _instance = new EventCollection();
        public static EventCollection Instance {
            get
            {
                return _instance;
            }
        }

        private Dictionary<int, string> Events = new Dictionary<int, string>()
        {
            { 10000, "Character xxx death" },
            { 10001, "The enemy withdrew" },
            { 10002, "Southern flood" }
        };

        public string Get(int key)
        {
            return Events.ContainsKey(key) ? Events[key] : string.Empty;
        }
    }
}
