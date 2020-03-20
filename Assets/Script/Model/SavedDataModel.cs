using System;

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
