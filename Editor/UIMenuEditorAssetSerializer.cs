#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public static class UIMenuEditorAssetSerializer
    {
        public static void Save(UIMenuData data, SimpleTreeView treeView)
        {
            string defaultPath = AssetDatabase.GetAssetPath(data);
            if (string.IsNullOrEmpty(defaultPath))
                defaultPath = Application.dataPath;

            string fullPath = EditorUtility.SaveFilePanelInProject(
                "Save ScriptableObject",
                "UIMenuData_" + data.Name,
                "asset",
                "Specify where to save the ScriptableObject.",
                defaultPath);

            if (string.IsNullOrEmpty(fullPath))
                return;

            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string directory = Path.GetDirectoryName(fullPath);
            string scriptableObjectDirectory = Path.Combine(directory, fileName);

            data.name = fileName;

            MoveDataAndChildDirectoryIfExists(data, directory, fileName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            CleanUnusedAssetsInDirectory(treeView, scriptableObjectDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SaveScriptableObject(data, directory, fileName);
            SaveTreeViewToDirectory(treeView, scriptableObjectDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetDataRootReferences(data, scriptableObjectDirectory);
            SetCategoryDataReferencesRecursivly(scriptableObjectDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void MoveDataAndChildDirectoryIfExists(UIMenuData data, string newDirectory, string newFileName)
        {
            string oldPath = AssetDatabase.GetAssetPath(data);
            if (string.IsNullOrEmpty(oldPath))
                return;

            string oldDirectory = Path.GetDirectoryName(oldPath);
            string oldFileName = Path.GetFileNameWithoutExtension(oldPath);

            if (oldDirectory.Equals(newDirectory) && oldFileName.Equals(newFileName))
                return;

            if (!AssetDatabase.IsValidFolder(newDirectory))
                AssetDatabase.CreateFolder(Path.GetDirectoryName(newDirectory), Path.GetFileName(newDirectory));

            string newPath = Path.Combine(newDirectory, newFileName + ".asset");
            string error = AssetDatabase.MoveAsset(oldPath, newPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to move asset to '{newPath}': {error}");
                return;
            }

            string oldScriptableObjectDirectory = Path.Combine(oldDirectory, oldFileName);
            string newScriptableObjectDirectory = Path.Combine(newDirectory, newFileName);
            if (AssetDatabase.IsValidFolder(oldScriptableObjectDirectory))
            {
                error = AssetDatabase.MoveAsset(oldScriptableObjectDirectory, newScriptableObjectDirectory);
                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Failed to move child directory to '{newScriptableObjectDirectory}': {error}");
            }
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
            var fileName = GetUniqueName(item);

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

            var existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existingAsset == null)
                AssetDatabase.CreateAsset(instance, path);
            else
            {
                EditorUtility.CopySerialized(instance, existingAsset);
                EditorUtility.SetDirty(existingAsset);
            }
        }

        private static void SetDataRootReferences(UIMenuData data, string directory)
        {
            var assets = new List<ScriptableObject>();
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
                assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(file));

            data.Root = assets.ToArray();
        }

        private static void SetCategoryDataReferencesRecursivly(string directory)
        {
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
            {
                var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>(file);
                if (data == null)
                    continue;

                string childDirectory = Path.Combine(directory, data.name);
                if (!Directory.Exists(childDirectory))
                    return;

                var assets = new List<ScriptableObject>();
                foreach (var childFile in Directory.GetFiles(childDirectory, "*.asset"))
                    assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(childFile));

                data.GetType().GetField("Data")?.SetValue(data, assets.ToArray());

                SetCategoryDataReferencesRecursivly(childDirectory);
            }
        }

        private static void CleanUnusedAssetsInDirectory(SimpleTreeView treeView, string directory)
        {
            if (!Directory.Exists(directory))
                return;

            var allAssets = new HashSet<string>(Directory.GetFiles(directory, "*.asset", SearchOption.AllDirectories));
            var allDirectories = new HashSet<string>(Directory.GetDirectories(directory, "*", SearchOption.AllDirectories));

            var treePaths = new HashSet<string>();
            foreach (var child in treeView.RootItem.Children)
                GatherTreeAssetPathsRecursively(child, directory, treePaths);

            foreach (var assetPath in allAssets)
                if (!treePaths.Contains(assetPath))
                    AssetDatabase.DeleteAsset(assetPath);

            foreach (var directoryPath in allDirectories.OrderByDescending(d => d.Length))
                if (!treePaths.Contains(directoryPath))
                    DeleteDirectory(directoryPath);
        }

        private static void GatherTreeAssetPathsRecursively(SimpleTreeViewItem item, string directory, HashSet<string> paths)
        {
            var itemName = GetUniqueName(item);

            paths.Add(Path.Combine(directory, itemName));
            paths.Add(Path.Combine(directory, itemName + ".asset"));

            foreach (var child in item.Children)
                GatherTreeAssetPathsRecursively(child, Path.Combine(directory, itemName), paths);
        }

        private static string GetUniqueName(SimpleTreeViewItem item)
        {
            string name = item.UniqueName;

            var itemData = item.UserData as ScriptableObject;
            if (itemData != null)
            {
                name += $" {(itemData as UIGeneratorTypeTemplate).ID}";
                itemData.name = name;
            }

            if (string.IsNullOrEmpty(name))
                name = "FALLBACK";

            return name;
        }

        public static void DeleteDirectory(string path)
        {
            string[] filePaths = Directory.GetFiles(path);
            string[] directoriesPaths = Directory.GetDirectories(path);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            foreach (string directoryPath in directoriesPaths)
                DeleteDirectory(directoryPath);

            if (!AssetDatabase.IsValidFolder(path))
                return;

            AssetDatabase.DeleteAsset(path);
        }
    }
}
#endif