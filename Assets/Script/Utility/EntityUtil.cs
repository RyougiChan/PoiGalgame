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

        public static T GetDeepValue<T>(object target, string fieldName)
        {
            T result = default(T);
            foreach (FieldInfo f in target.GetType().GetFields())
            {
                if (f.Name.Equals(fieldName))
                {
                    return GetValue<T>(target, fieldName);
                }
                else if (f.FieldType.Namespace.StartsWith("Assets.Script.Model"))
                {
                    result = GetDeepValue<T>(f.GetValue(target), fieldName);
                    if (!result.Equals(default(T)))
                    {
                        return result;
                    }
                }
            }
            return result;
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

        public static List<FieldInfo> GetTypeFields<T>(object target, List<FieldInfo> r)
        {

            foreach (FieldInfo f in target.GetType().GetFields())
            {
                if (f.FieldType.Equals(typeof(T)))
                {
                    r.Add(f);
                }
                else if (f.FieldType.Namespace.StartsWith("Assets.Script.Model"))
                {
                    GetTypeFields<T>(f.GetValue(target), r);
                }
            }

            return r;
        }

    }
}
