using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.Script.Utility
{
    public class EntityUtil
    {
        public static bool SetDeepValue(object target, string fieldName, object valueToSet)
        {
            if(SetValue(target, fieldName, valueToSet)) return true;
            foreach(FieldInfo f in target.GetType().GetFields())
            {
                if(f.FieldType.Namespace.StartsWith("Assets.Script.Model")) {
                    if(SetDeepValue(f.GetValue(target), fieldName, valueToSet)) return true;
                }
            }
            return false;
        }

        public static bool SetValue(object target, string fieldName, object valueToSet)
        {
            if(null != target)
            {
                FieldInfo fieldInfo = target.GetType().GetField(fieldName);
                if (null != fieldInfo)
                {
                    fieldInfo.SetValue(target, Convert.ChangeType(valueToSet, fieldInfo.FieldType));
                    return true;
                }
            }
            return false;
        }

        public static T GetValue<T>(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName);
            return null == fieldInfo ? default(T) : (T)target.GetType().GetField(fieldName).GetValue(target);
        }
    }
}
