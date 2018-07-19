using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        internal static IEnumerable<GameObject> GetAncestors(this GameObject obj)
        {
            var iter = obj.transform.parent;
            while (iter != null)
            {
                yield return iter.gameObject;

                iter = iter.parent;
            }
        }

        internal static IEnumerable<GameObject> GetDescendants(this GameObject obj)
        {
            for (int temp = 0; temp < obj.transform.childCount; temp++)
            {
                var child = obj.transform.GetChild(temp).gameObject;
                yield return child;

                if (child.transform.childCount > 0)
                {
                    foreach (var childEnum in child.GetDescendants())
                    {
                        yield return childEnum;
                    }
                }
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains parents of every GameObject from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Parents(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                if (item.transform.parent)
                    yield return item.transform.parent.gameObject;
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains every GameObject from the source collection and their parents</summary>
        ///
        public static IEnumerable<GameObject> ParentsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                yield return item;

                if (item.transform.parent)
                    yield return item.transform.parent.gameObject;
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains ancestors of every GameObject from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var parent in GetAncestors(item))
                {
                    yield return parent;
                }
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains every GameObject from the source collection and their ancestors</summary>
        ///
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                yield return item;

                foreach (var parent in GetAncestors(item))
                {
                    yield return parent;
                }
            }
        }


        ///
        /// <summary>Returns a collection of GameObjects that contains all children of every GameObject from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (Transform child in item.transform)
                {
                    yield return child.gameObject;
                }
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains every GameObject from the source collection and their children</summary>
        ///
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                yield return item;

                foreach (Transform child in item.transform)
                {
                    yield return child.gameObject;
                }
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains descendants of every GameObject from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var child in item.GetDescendants())
                {
                    yield return child;
                }
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects that contains every GameObject from the source collection and their descendants</summary>
        ///
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                yield return item;

                foreach (var child in item.GetDescendants())
                {
                    yield return child;
                }
            }
        }


        ///
        /// <summary>Returns a collection of all Components from every GameObject from the source collection</summary>
        ///
        public static IEnumerable<T> Components<T>(this IEnumerable<GameObject> objects) where T : Component
        {
            foreach (var obj in objects)
            {
                var components = obj.GetComponents<T>();
                foreach (var cmp in components)
                {
                    yield return cmp;
                }
            }
        }

        ///
        /// <summary>Returns a collection of SerializedProperties with all properties for each GameObject int the source collection</summary>
        ///
        public static IEnumerable<SerializedProperty> Properties(this IEnumerable<GameObject> objects)
        {
            foreach (var obj in objects)
            {
                var serializedObj = new SerializedObject(obj);
                var iterator = serializedObj.GetIterator();
                while (iterator.NextVisible(true))
                {
                    yield return iterator.Copy();
                }

                // List all component properties as well
                var array = serializedObj.FindProperty("m_Component.Array");
                if (array != null && array.isArray)
                {
                    for (int temp = 0; temp < array.arraySize; ++temp)
                    {
                        var component = array.GetArrayElementAtIndex(temp).FindPropertyRelative("component");
                        if (component == null)
                            continue;

                        var componentRef = component.objectReferenceValue;
                        if (componentRef == null)
                            continue;

                        var componentSO = new SerializedObject(componentRef);
                        var iteratorCmp = componentSO.GetIterator();
                        while (iteratorCmp.NextVisible(true))
                        {
                            yield return iteratorCmp.Copy();
                        }
                    }
                }
            }
        }


        ///
        /// <summary>Returns a collection of GameObjects with names that is/isn't contain specified string based on condition</summary>
        ///
        public static IEnumerable<GameObject> WhereName(this IEnumerable<GameObject> source, string str, Condition condition = Condition.Contain)
        {
            return source.Where(GetNamePredicate<GameObject>(condition, (x) => x.name, str));
        }

        ///
        /// <summary>Returns a collection of GameObjects with tags that is/isn't contain specified string based on condition</summary>
        ///
        public static IEnumerable<GameObject> WhereTag(this IEnumerable<GameObject> source, string str, Condition condition = Condition.Contain)
        {
            return source.Where(GetNamePredicate<GameObject>(condition, (x) => x.tag, str));
        }

        ///
        /// <summary>Returns a collection of GameObjects which has Component</summary>
        ///
        public static IEnumerable<GameObject> WhereHasComponent<T>(this IEnumerable<GameObject> source) where T : Component
        {
            return source.Components<T>().GameObjects().Distinct();
        }


        ///
        /// <summary>Destroy every GameObject in the source collection</summary>
        ///
        public static void Destroy(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                Object.DestroyImmediate(item);
            }
        }
    }
}