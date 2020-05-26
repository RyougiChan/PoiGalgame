using Assets.Script.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public int ExampleOtherValue = -9999;

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

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(GameValues))) return false;

            GameValues gameValues = obj as GameValues;

            List<FieldInfo> typeFields = new List<FieldInfo>();
            EntityUtil.GetTypeFields<int>(GlobalGameData.GameValues, typeFields);

            foreach (FieldInfo f in typeFields)
            {
                string fieldName = f.Name;
                int v1 = EntityUtil.GetDeepValue<int>(gameValues, fieldName);
                if(v1 != -9999)
                {
                    int v2 = EntityUtil.GetDeepValue<int>(GlobalGameData.GameValues, fieldName);
                    if(v1 != v2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
