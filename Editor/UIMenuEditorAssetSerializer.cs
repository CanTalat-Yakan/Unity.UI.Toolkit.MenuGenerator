#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public static class UIMenuEditorAssetSerializer
    {
        public static void Save(UIMenuData data, SimpleTreeView treeView)
        {
            string fullPath = EditorUtility.SaveFilePanelInProject(
                "Save ScriptableObject",
                "UIMenuData_" + data.Name,
                "asset",
                "Specify where to save the ScriptableObject.");

            if (string.IsNullOrEmpty(fullPath))
                return;

            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string directory = Path.GetDirectoryName(fullPath);
            string childDirectory = Path.Combine(directory, fileName);
            CleanDirectory(childDirectory);

            data.name = fileName;

            SaveScriptableObject(data, directory, fileName);
            SaveTreeViewToDirectory(treeView, childDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetDataRootRefernces(fileName, directory);
            SetCategoryDataReferencesRecursivly(fileName, directory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CleanDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var file in Directory.GetFiles(directory, "*.asset"))
                AssetDatabase.DeleteAsset(file);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            foreach (var subDirectory in Directory.GetDirectories(directory))
                Directory.Delete(subDirectory, true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        private static void SaveTreeViewToDirectory(SimpleTreeView treeView, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (var item in treeView.RootItem.Children)
                SaveTreeViewDataRecursively(item, directory);
        }

        private static void SaveTreeViewDataRecursively(SimpleTreeViewItem item, string directory)
        {
            string fileName = item.UniqueName;

            var itemData = item.UserData as ScriptableObject;
            if(string.IsNullOrEmpty(fileName))
                fileName = itemData?.name;

            if(string.IsNullOrEmpty(fileName))
                fileName = "FALLBACK_NAME_NULL";

            if (item?.UserData is ScriptableObject scriptableObject)
                SaveScriptableObject(scriptableObject, directory, fileName);

            foreach (var child in item.Children)
                SaveTreeViewDataRecursively(child, Path.Combine(directory, fileName));
        }

        private static void SaveScriptableObject<T>(T instance, string directory, string fileName) where T : ScriptableObject
        {
            if (instance == null || string.IsNullOrEmpty(directory))
                return;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, fileName + ".asset");

            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existingAsset == null)
                AssetDatabase.CreateAsset(instance, path);
            else
            {
                EditorUtility.CopySerialized(instance, existingAsset);
                EditorUtility.SetDirty(existingAsset);
            }
        }

        private static void SetDataRootRefernces(string fileName, string directory)
        {
            var assetPath = Path.Combine(directory, fileName + ".asset");
            var data = AssetDatabase.LoadAssetAtPath<UIMenuData>(assetPath);
            List<ScriptableObject> assets = new();
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
                assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(file));
            data.Root = assets.ToArray();
        }

        private static void SetCategoryDataReferencesRecursivly(string fileName, string directory)
        {
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
            {
                var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>(file);
                if (data == null)
                    continue;

                if (data is UIMenuCategoryData category)
                {
                    List<ScriptableObject> assets = new();
                    foreach (var childFile in Directory.GetFiles(Path.Combine(directory, category.Name), "*.asset"))
                        assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(childFile));
                    category.Data = assets.ToArray();
                    EditorUtility.SetDirty(category);
                }
            }
        }
    }
}
#endif