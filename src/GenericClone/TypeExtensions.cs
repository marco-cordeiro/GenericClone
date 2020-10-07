#nullable enable
using System;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace GenericClone
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface<T>(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(T));
        }

        public static bool IsEnumerable(this Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }

        public static bool IsCollection(this Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }

        public static bool IsList(this Type type)
        {
            return (type.GetInterface(nameof(IList)) != null);
        }

        internal static MethodInfo GetCloneMethod()
        {
            return typeof(object).GetMethod(nameof(MemberwiseClone), BindingFlags.Instance | BindingFlags.NonPublic)!;
        }

    }
}
