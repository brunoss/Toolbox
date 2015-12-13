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
        private static readonly HashSet<Type> _NumericTypes;
        private static readonly HashSet<Type> _ScalarTypes; 
        static ReflectionUtils()
        {
            _NumericTypes = new HashSet<Type>();
            _NumericTypes.Add(typeof(byte));
            _NumericTypes.Add(typeof(sbyte));
            _NumericTypes.Add(typeof(short));
            _NumericTypes.Add(typeof(ushort));
            _NumericTypes.Add(typeof(int));
            _NumericTypes.Add(typeof(uint));
            _NumericTypes.Add(typeof(long));
            _NumericTypes.Add(typeof(ulong));
            _NumericTypes.Add(typeof(float));
            _NumericTypes.Add(typeof(double));
            _NumericTypes.Add(typeof(decimal));

            _ScalarTypes = new HashSet<Type>(_NumericTypes);
            _ScalarTypes.Add(typeof (byte[]));
            _ScalarTypes.Add(typeof(DateTime));
            _ScalarTypes.Add(typeof(DateTime?));
            _ScalarTypes.Add(typeof(string));
            _ScalarTypes.Add(typeof(byte?));
            _ScalarTypes.Add(typeof(sbyte?));
            _ScalarTypes.Add(typeof(short?));
            _ScalarTypes.Add(typeof(ushort?));
            _ScalarTypes.Add(typeof(int?));
            _ScalarTypes.Add(typeof(uint?));
            _ScalarTypes.Add(typeof(long?));
            _ScalarTypes.Add(typeof(ulong?));
            _ScalarTypes.Add(typeof(float?));
            _ScalarTypes.Add(typeof(double?));
            _ScalarTypes.Add(typeof(decimal?));
            _ScalarTypes.Add(typeof(bool));
            _ScalarTypes.Add(typeof(bool?));
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
                var ret = mi.DeclaringType.GetProperty(name);
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
            var builder = new StringBuilder();
            string name = type.Name.Substring(0, type.Name.LastIndexOf('`'));
            builder.Append(name);
            builder.Append('<');
            var names = type.GenericTypeArguments.Select(GetName);
            builder.Append(string.Join(",", names));
            builder.Append('>');
            return builder.ToString();
        }

        public static bool IsNumeric(this Type type)
        {
            return _NumericTypes.Contains(type);
        }

        public static bool IsScalar(this Type type)
        {
            return _ScalarTypes.Contains(type);
        }

        public static T ShallowCopyTo<T>(this T from, T to)
        {
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                if (property.SetMethod != null && property.GetMethod != null)
                    property.SetMethod.Invoke(to, new[] { property.GetMethod.Invoke(from, null) });
            }
            return to;
        }
    }
}
