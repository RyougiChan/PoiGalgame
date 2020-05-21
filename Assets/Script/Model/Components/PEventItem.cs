using Assets.Script.Model.Datas;
using System;

namespace Assets.Script.Model.Components
{
    [Serializable]
    public class PEventItem : PComponent
    {
        public string EvtId;
        public GameValues DeltaGameValues;
    }
}
