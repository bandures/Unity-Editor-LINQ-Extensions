using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        ///
        /// <summary>Returns a collection of all loaded Scenes</summary>
        ///
        public static IEnumerable<Scene> AllScenes()
        {
            for (int temp = 0; temp < SceneManager.sceneCount; temp++)
            {
                yield return SceneManager.GetSceneAt(temp);
            }
        }

        ///
        /// <summary>Returns a collection of Scenes specified in build settings</summary>
        ///
        public static IEnumerable<Scene> BuildScenes()
        {
            for (int temp = 0; temp < SceneManager.sceneCountInBuildSettings; temp++)
            {
                yield return SceneManager.GetSceneByBuildIndex(temp);
            }
        }


        ///
        /// <summary>Returns a collection of root GameObjects from every Scene from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Roots(this IEnumerable<Scene> source)
        {
            foreach (var item in source)
            {
                var roots = item.GetRootGameObjects();
                foreach (var obj in roots)
                {
                    yield return obj;
                }
            }
        }

        ///
        /// <summary>Returns a collection of all GameObjects from every Scene from the source collection</summary>
        ///
        public static IEnumerable<GameObject> Descendants(this IEnumerable<Scene> source)
        {
            foreach (var item in source)
            {
                var roots = item.GetRootGameObjects();
                foreach (var obj in roots)
                {
                    yield return obj;

                    foreach (var child in obj.GetDescendants())
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}