using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.Script.Utility
{
    public class EntityUtil
    {

        public static void SetValue(object target, string fieldName, object valueToSet)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName);
            if (null != fieldInfo)
            {
                fieldInfo.SetValue(target, valueToSet);
            }
        }

        public static T GetValue<T>(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName);
            return null == fieldInfo ? default(T) : (T)target.GetType().GetField(fieldName).GetValue(target);
        }
    }
}
