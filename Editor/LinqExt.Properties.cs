using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        ///
        /// <summary>Returns a collection of all GameObjects associated with every Property in the source collection</summary>
        ///
        public static IEnumerable<GameObject> GameObjects(this IEnumerable<SerializedProperty> source)
        {
            foreach (var item in source)
            {
                if (item.serializedObject.isEditingMultipleObjects)
                {
                    Debug.LogWarningFormat("UQL doesn't support SerializedProperty with multiple objects: {0}", item.propertyPath);
                    continue;
                }

                var obj = item.serializedObject.targetObject;
                if (obj is GameObject)
                    yield return obj as GameObject;
                else if (obj is Component)
                    yield return (obj as Component).gameObject;
                else
                {
                    Debug.LogWarningFormat("UQL doesn't support SerializedProperty for type {0} ({1})", obj.GetType().Name, item.propertyPath);
                    continue;
                }
            }
        }

        //
        // Filter GameObjects by name
        //
        public static IEnumerable<SerializedProperty> WhereName(this IEnumerable<SerializedProperty> objects, string str, Condition condition = Condition.Contain)
        {
            return objects.Where(GetNamePredicate<SerializedProperty>(condition, (x) => x.name, str));
        }
    }
}