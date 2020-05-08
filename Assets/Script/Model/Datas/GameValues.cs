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
        public RoleStatus RoleStatus = new RoleStatus();
        public RoleAbility RoleAbility = new RoleAbility();
        public RoleMood RoleMood = new RoleMood();

        // ...
        public int ExampleOtherValue;

        public string ToJSONString()
        {
            return new StringBuilder()
                .Append("{")
                    .Append("\"HealthPoint\":").Append(this.RoleStatus.HealthPoint).Append(",")
                    .Append("\"ManaPoint\":").Append(this.RoleStatus.ManaPoint).Append(",")
                    .Append("\"ExperiencePoint\":").Append(this.RoleStatus.ExperiencePoint).Append(",")
                    .Append("\"SkillPoint\":").Append(this.RoleStatus.SkillPoint).Append(",")
                    .Append("\"Attack\":").Append(this.RoleAbility.Attack).Append(",")
                    .Append("\"Defence\":").Append(this.RoleAbility.Defence).Append(",")
                    .Append("\"Evasion\":").Append(this.RoleAbility.Evasion).Append(",")
                    .Append("\"ExampleOtherValue\":").Append(this.ExampleOtherValue)
                .Append("}")
                .ToString();
        }
    }
}
