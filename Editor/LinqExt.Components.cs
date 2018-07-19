using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        ///
        /// <summary>Returns a collection of all GameObjects associated with every Component in the source collection</summary>
        ///
        public static IEnumerable<GameObject> GameObjects(this IEnumerable<Component> source)
        {
            foreach (var item in source)
            {
                yield return item.gameObject;
            }
        }

        ///
        /// <summary>Returns a collection of all SerializedProperties for every Component in the source collection</summary>
        ///
        public static IEnumerable<SerializedProperty> Properties(this IEnumerable<Component> components)
        {
            foreach (var cmp in components)
            {
                var serializedObj = new SerializedObject(cmp);
                var iterator = serializedObj.GetIterator();
                while (iterator.NextVisible(true))
                {
                    yield return iterator.Copy();
                }
            }
        }


        ///
        /// <summary>Returns a collection of Components with tags that is/isn't contain specified string based on condition</summary>
        ///
        public static IEnumerable<T> WhereTag<T>(this IEnumerable<T> objects, string str, Condition condition = Condition.Contain) where T : Component
        {
            return objects.Where(GetNamePredicate<T>(condition, (x) => x.tag, str));
        }
    }
}