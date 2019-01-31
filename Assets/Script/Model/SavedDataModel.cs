using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    [Serializable]
    public class SavedDataModel
    {
        public int savedDataIndex;
        public DateTime savedTime;
        // TODO: Remove it, replace it with `GalgameAction` or other
        public GalgameActionForSavedData galgameAction;
        public int galgameActionIndex;
    }
}
