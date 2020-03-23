using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model.Datas
{
    [Serializable]
    public class GameValues
    {
        // Commond RPG game values
        public int HealthPoint;
        public int ManaPoint;
        public int ExperiencePoint;
        public int SkillPoint;

        // ...
        public int ExampleOtherValue;

        public string ToJSONString()
        {
            return new StringBuilder()
                .Append("{")
                    .Append("\"HealthPoint\":").Append(this.HealthPoint).Append(",")
                    .Append("\"ManaPoint\":").Append(this.ManaPoint).Append(",")
                    .Append("\"ExperiencePoint\":").Append(this.ExperiencePoint).Append(",")
                    .Append("\"SkillPoint\":").Append(this.SkillPoint).Append(",")
                    .Append("\"ExampleOtherValue\":").Append(this.ExampleOtherValue)
                .Append("}")
                .ToString();
        }
    }
}
