using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        public class Asset
        {
            public string guid;
            public string path;
        }

        internal static IEnumerable<Asset> GetAssets(string filter = "t:Object")
        {
            var assets = AssetDatabase.FindAssets(filter);
            foreach (var asset in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                yield return new Asset() { guid = asset, path = path };
            }
        }

        ///
        /// <summary>Returns a collection of all assets in the project</summary>
        ///
        public static IEnumerable<Asset> AllAssets()
        {
            return GetAssets("t:Object");
        }

        ///
        /// <summary>Returns a collection of all assets of specific type in the project</summary>
        ///
        public static IEnumerable<Asset> AllAssets<T>() where T : UnityEngine.Object
        {
            return GetAssets("t:" + typeof(T).Name);
        }

        ///
        /// <summary>Returns a collection of all models in the project</summary>
        ///
        public static IEnumerable<Asset> AllModels()
        {
            return GetAssets("t:Model");
        }

        ///
        /// <summary>Returns a collection of all textures in the project</summary>
        ///
        public static IEnumerable<Asset> AllTextures()
        {
            return GetAssets("t:Texture");
        }


        ///
        /// <summary>Returns a collection of AssetImporters for every asset in the source collection</summary>
        ///
        public static IEnumerable<T> AssetImporters<T>(this IEnumerable<Asset> assets) where T : AssetImporter
        {
            foreach (var asset in assets)
            {
                yield return AssetImporter.GetAtPath(asset.path) as T;
            }
        }

        ///
        /// <summary>Returns a collection of ScriptableObjects for every ScriptableObject asset in the source collection (non-ScriptableObjects are ignored)</summary>
        ///
        public static IEnumerable<T> WhereType<T>(this IEnumerable<Asset> assets) where T : ScriptableObject
        {
            foreach (var asset in assets)
            {
                var value = AssetDatabase.LoadMainAssetAtPath(asset.path) as T;
                if (value != null)
                    yield return value;
            }
        }

        ///
        /// <summary>Returns a collection of GameObjects for every asset in the source collection (non-GameObjects are ignored)</summary>
        ///
        public static IEnumerable<GameObject> WhereType(this IEnumerable<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var value = AssetDatabase.LoadMainAssetAtPath(asset.path) as GameObject;
                if (value != null)
                    yield return value;
            }
        }

        ///
        /// <summary>Returns a collection of assets with a path matching string and condition from the source collection</summary>
        ///
        public static IEnumerable<Asset> WherePath(this IEnumerable<Asset> source, string str, Condition condition = Condition.Contain)
        {
            return source.Where(GetNamePredicate<Asset>(condition, (x) => x.path, str));
        }


        ///
        /// <summary>Returns a collection of SerializedProperties witl all properties for each asset in the source collection</summary>
        ///
        public static IEnumerable<SerializedProperty> Properties(this IEnumerable<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var serializedObj = new SerializedObject(AssetImporter.GetAtPath(asset.path));
                var iterator = serializedObj.GetIterator();
                while (iterator.NextVisible(true))
                {
                    yield return iterator.Copy();
                }
            }
        }
    }
}