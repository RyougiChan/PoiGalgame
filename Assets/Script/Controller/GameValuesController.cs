using Assets.Script.Model.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Controller
{
    public class GameValuesController : MonoBehaviour
    {
        private void Start()
        {
            if (null == GlobalGameData.GameValues) GlobalGameData.GameValues = new GameValues();
            if (null == GlobalGameData.GameValues.RoleAbility) GlobalGameData.GameValues.RoleAbility = new RoleAbility();
            if (null == GlobalGameData.GameValues.RoleStatus) GlobalGameData.GameValues.RoleStatus = new RoleStatus();
        }

        private void Update()
        {

        }

        public void UpdateGlobalGameValues(GameValues gameValues)
        {
            if (null != gameValues.RoleAbility)
            {
                RoleAbility ability = GlobalGameData.GameValues.RoleAbility;
                ability.Attack += gameValues.RoleAbility.Attack;
                ability.Defence += gameValues.RoleAbility.Defence;
                ability.Evasion += gameValues.RoleAbility.Evasion;
            }
            if (null != gameValues.RoleStatus)
            {
                RoleStatus status = GlobalGameData.GameValues.RoleStatus;
                status.HealthPoint += gameValues.RoleStatus.HealthPoint;
                status.ManaPoint += gameValues.RoleStatus.ManaPoint;
                status.SkillPoint += gameValues.RoleStatus.SkillPoint;
                status.ExperiencePoint += gameValues.RoleStatus.ExperiencePoint;
            }
            GlobalGameData.GameValues.ExampleOtherValue += gameValues.ExampleOtherValue;
        }
    }
}
