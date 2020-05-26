using Assets.Script.Model.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    public class DefaultValues
    {
        public static RoleStatus DEFAULT_ROLESTATUS = new RoleStatus()
        {
            ExperiencePoint = 0,
            HealthPoint = 100,
            ManaPoint = 0,
            SkillPoint = 0
        };
        public static RoleAbility DEFAULT_ROLEABILITY = new RoleAbility()
        {
            Attack = 1,
            Defence = 1,
            Evasion = 1
        };
        public static RoleMood DEFAULT_ROLEMOOD = new RoleMood()
        {
            Angry = 0,
            Hate = 0,
            Sorrow = 0
        };
        public static GameValues DEFAULT_GAMEVALUES = new GameValues() {
            ExampleOtherValue = 0,
            RoleAbility = DEFAULT_ROLEABILITY,
            RoleStatus = DEFAULT_ROLESTATUS,
            RoleMood = DEFAULT_ROLEMOOD
        };
    }
}
