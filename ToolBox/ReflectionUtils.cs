using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ToolBox
{
    public static class ReflectionUtils
    {
        private static readonly HashSet<Type> numericTypes;
        private static readonly HashSet<Type> scalarTypes; 
        static ReflectionUtils()
        {
            numericTypes = new HashSet<Type>();
            numericTypes.Add(typeof(byte));
            numericTypes.Add(typeof(sbyte));
            numericTypes.Add(typeof(short));
            numericTypes.Add(typeof(ushort));
            numericTypes.Add(typeof(int));
            numericTypes.Add(typeof(uint));
            numericTypes.Add(typeof(long));
            numericTypes.Add(typeof(ulong));
            numericTypes.Add(typeof(float));
            numericTypes.Add(typeof(double));
            numericTypes.Add(typeof(decimal));

            scalarTypes = new HashSet<Type>(numericTypes);
            scalarTypes.Add(typeof (byte[]));
            scalarTypes.Add(typeof(DateTime));
            scalarTypes.Add(typeof(DateTime?));
            scalarTypes.Add(typeof(string));
            scalarTypes.Add(typeof(byte?));
            scalarTypes.Add(typeof(sbyte?));
            scalarTypes.Add(typeof(short?));
            scalarTypes.Add(typeof(ushort?));
            scalarTypes.Add(typeof(int?));
            scalarTypes.Add(typeof(uint?));
            scalarTypes.Add(typeof(long?));
            scalarTypes.Add(typeof(ulong?));
            scalarTypes.Add(typeof(float?));
            scalarTypes.Add(typeof(double?));
            scalarTypes.Add(typeof(decimal?));
            scalarTypes.Add(typeof(bool));
            scalarTypes.Add(typeof(bool?));
        }


        //http://kozmic.net/2009/02/23/castle-dynamic-proxy-tutorial-part-vi-handling-non-virtual-methods/
        public static bool IsPropertyGetter(this MethodInfo mi)
        {
            return mi.IsSpecialName && mi.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        public static bool IsPropertySetter(this MethodInfo mi)
        {
            return mi.IsSpecialName && mi.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        public static string GetPropertyName(this MethodInfo mi)
        {
            //note "get_".Lenght = "set_".Lenght
            return mi.Name.Substring("get_".Length);
        }

        public static bool IsOverriding(this MethodInfo mi)
        {
            return mi.GetBaseDefinition().DeclaringType != mi.DeclaringType;
        }

        public static PropertyInfo MethodToProperty(this MethodInfo mi)
        {
            if (mi.IsPropertyGetter() || mi.IsPropertySetter())
            {
                string name = mi.GetPropertyName();
                Debug.Assert(mi.DeclaringType != null);
                PropertyInfo ret = mi.DeclaringType.GetProperty(name);
                return ret;
            }
            return null;
        }

        public static string GetName(this Type type)
        {
            if (type.GenericTypeArguments.Length == 0)
            {
                return type.Name;
            }
            StringBuilder builder = new StringBuilder();
            string name = type.Name.Substring(0, type.Name.LastIndexOf('`'));
            builder.Append(name);
            builder.Append('<');
            var names = type.GenericTypeArguments.Select(GetName);
            builder.Append(String.Join(",", names));
            builder.Append('>');
            return builder.ToString();
        }

        public static bool IsNumeric(this Type type)
        {
            return numericTypes.Contains(type);
        }

        public static bool IsScalar(this Type type)
        {
            return scalarTypes.Contains(type);
        }

        public static T ShallowCopyTo<T>(this T from, T to)
        {
            Type type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                if (property.SetMethod != null && property.GetMethod != null)
                    property.SetMethod.Invoke(to, new[] { property.GetMethod.Invoke(from, null) });
            }
            return to;
        }
    }
}
