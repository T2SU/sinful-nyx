using System;
using UnityEngine;

namespace Sevens.Utils
{
    [Serializable]
    public class NamedObject<T>
    {
        public string Name;
        public T Object;

        public static implicit operator T(NamedObject<T> obj)
            => obj != null ? obj.Object : default;
    }

    public static class NamedObjectExtensions
    {
        public static NamedObject<T> FindByName<T>(this NamedObject<T>[] array, string key)
        {
            foreach (var item in array)
                if (item.Name == key)
                    return item;
            return null;
        }
    }
}
