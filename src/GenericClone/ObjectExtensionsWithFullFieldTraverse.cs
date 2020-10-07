#nullable enable
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GenericClone
{
    public static class ObjectExtensionsWithFullFieldTraverse
    {
        private static readonly MethodInfo CloneMethod = TypeExtensions.GetCloneMethod();
        private static readonly MethodInfo GenericCollectionCloneMethod = GetGenericCollectionCloneMethod();
        private static readonly IDictionary<Type, IEnumerable<FieldInfo>> FieldInfoCache = new Dictionary<Type, IEnumerable<FieldInfo>>();

        public static T ShallowClone2<T>(this T instance) where T : class
        {
            var clone = CloneMethod.Invoke(instance, null);
            return (T)clone;
        }

        public static T DeepClone2<T>(this T instance) where T : class
        {
            return DeepObjectClone(instance);
        }        
        
        private static T DeepObjectClone<T>(this T instance)
        {
            if (instance is null)
                return instance;

            return (T)DeepCloneObject(instance);
        }

        private static object DeepCloneObject(this object instance)
        {
            var instanceType = instance.GetType();

            //if the instance is a value type or string just return the value
            if (instanceType.IsValueType || instance is string)
                return instance;

            // if the instance is a collection clone then special logic is required
            if (instanceType.ImplementsInterface<IEnumerable>())
                return DeepCloneCollection((IEnumerable)instance);

            var clone = Activator.CreateInstance(instanceType);

            var fields = instanceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var value = field.GetValue(instance);

                //no need to do anything if the field value is null
                if (value is null)
                    continue;

                //otherwise we just clone it
                field.SetValue(clone, value.DeepCloneObject());
            }

            return clone;
        }

        private static IEnumerable<FieldInfo> GetFields(Type instanceType)
        {
            if (FieldInfoCache.TryGetValue(instanceType, out var fields))
                return fields;

            fields = instanceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfoCache.Add(instanceType, fields);

            return fields;
        }

        private static object DeepCloneCollection(this IEnumerable instance)
        {
            var argumentType = instance.GetType().GetGenericArguments().First();
            
            var typedMethod = GenericCollectionCloneMethod.MakeGenericMethod(argumentType);

            return typedMethod.Invoke(null, new object[] {instance});
        }
        
        private static object DeepCloneCollection<T>(IEnumerable<T> instance)
        {
            var items =  from T item in instance select item.DeepObjectClone();

            if (instance.GetType().IsArray)
                return items.ToArray();

            return Activator.CreateInstance(instance.GetType(), new object[] {items});
        }

        private static MethodInfo GetGenericCollectionCloneMethod()
        {
            return typeof(ObjectExtensionsWithFullFieldTraverse).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .First(x => x.Name == nameof(DeepCloneCollection) && x.IsGenericMethod).GetGenericMethodDefinition();
        }
    }
}
