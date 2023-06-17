using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TASMod
{
    public class Reflector
    {
        public static Dictionary<string, FieldInfo> FieldInfos = new Dictionary<string, FieldInfo>();
        public static Dictionary<string, PropertyInfo> PropertyInfos = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();
        public static Dictionary<string, List<string>> FieldsInTypeInfos = new Dictionary<string, List<string>>();

        public const BindingFlags AllFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        public const BindingFlags HiddenFlags = (BindingFlags.NonPublic | BindingFlags.Instance);

        public static FieldInfo GetField<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = type.ToString() + "__" + field;
            if (FieldInfos.ContainsKey(key))
            {
                return FieldInfos[key];
            }
            FieldInfo info = type.GetType().GetField(field, flags);
            FieldInfos.Add(key, info);
            return info;
        }

        public static PropertyInfo GetProperty<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = type.ToString() + "__" + field;
            if (PropertyInfos.ContainsKey(key))
            {
                return PropertyInfos[key];
            }
            PropertyInfo info = type.GetType().GetProperty(field, flags);
            PropertyInfos.Add(key, info);
            return info;
        }
        public static MethodInfo GetMethod<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = type.ToString() + "__" + field;
            if (MethodInfos.ContainsKey(key))
            {
                return MethodInfos[key];
            }
            MethodInfo info = type.GetType().GetMethod(field, flags);
            MethodInfos.Add(key, info);
            return info;
        }


        public static V GetValue<T, V>(T obj, string field, BindingFlags flags = HiddenFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                V value = (V)info.GetValue(obj);
                return value;
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    V value = (V)pinfo.GetValue(obj, null);
                    return value;
                }
            }
            return default(V);
        }

        public static Type GetValueType<T>(T obj, string field, BindingFlags flags = AllFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                return info.FieldType;
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    return pinfo.PropertyType;
                }
            }
            return null;
        }

        public static object GetValue<T>(T obj, string field, BindingFlags flags = AllFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                return info.GetValue(obj);
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    return pinfo.GetValue(obj, null);
                }
            }
            return null;
        }

        public static void SetValue<T, V>(T obj, string field, V val, BindingFlags flags = HiddenFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                info.SetValue(obj, val);
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    pinfo.SetValue(obj, val, null);
                }
            }
        }

        public static void InvokeMethod<T>(T obj, string field, object[] args = null, BindingFlags flags = HiddenFlags)
        {
            MethodInfo info = GetMethod(obj, field, flags);
            if (info != null)
            {
                info.Invoke(obj, args);
                return;
            }

            throw new MethodAccessException(string.Format("Method does not exist: {0}::{1}", typeof(T).Name, field));
        }

        public static V InvokeMethod<T, V>(T obj, string field, object[] args = null, BindingFlags flags = HiddenFlags)
        {
            MethodInfo info = GetMethod(obj, field, flags);
            if (info != null)
            {
                return (V)info.Invoke(obj, args);
            }

            throw new MethodAccessException(string.Format("Method does not exist: {0}::{1}", typeof(T).Name, field));
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string space)
        {
            return assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith(space, StringComparison.Ordinal)).ToArray();
        }

        public static Type[] AllTypesInAssembly(Assembly assembly)
        {
            return assembly.GetTypes().ToArray();
        }

        public static Type GetTypeInAssembly(Assembly assembly, string fullname)
        {
            var matches = assembly.GetTypes().Where(t => t.FullName == fullname);
            foreach (var v in AllTypesInAssembly(assembly))
            {
                ModEntry.Console.Log($"{v.FullName}");
            }
            foreach (var t in matches)
            {
                return t;
            }
            return null;
        }

        public static IEnumerable<string> GetFields<T>(Type type, BindingFlags flags = AllFlags)
        {
            string key = type.ToString() + "_" + typeof(T).ToString();
            if (FieldsInTypeInfos.ContainsKey(key))
            {
                return FieldsInTypeInfos[key];
            }
            List<string> fields = new List<string>(
                type
                .GetFields(flags)
                .Where(f => f.FieldType.Name == typeof(T).Name)
                .Select(f => f.Name)
            );
            FieldsInTypeInfos.Add(key, fields);
            return fields;
        }

        public static IEnumerable<string> GetMethods(Type type, BindingFlags flags = AllFlags)
        {
            List<string> fields = new List<string>(
                type
                .GetMethods(flags)
                .Select(f => f.Name)
            );
            return fields;
        }

        public static IEnumerable<string> GetListFields<T>(Type type, BindingFlags flags = AllFlags)
        {
            string key = "List`1_" + type.ToString() + "_" + typeof(T).ToString();
            if (FieldsInTypeInfos.ContainsKey(key))
            {
                return FieldsInTypeInfos[key];
            }
            List<string> fields = new List<string>(
                type
                .GetFields(flags)
                .Where(f => f.FieldType.Name == "List`1" && f.FieldType.FullName.Contains(typeof(T).Name))
                .Select(f => f.Name)
            );
            FieldsInTypeInfos.Add(key, fields);
            return fields;
        }

        public static object GetDynamicCastField(object obj, string field)
        {
            try
            {
                Type type = obj.GetType();
                dynamic castObj = Convert.ChangeType(obj, type);
                return Reflector.GetValue(castObj, field);
            }
            catch
            {
                return null;
            }
        }
    }
}
