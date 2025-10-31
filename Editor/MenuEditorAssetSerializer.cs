#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public static class MenuEditorAssetSerializer
    {
        public static void Save(MenuData data, SimpleTreeView treeView)
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

            SaveScriptableObject(data, directory, fileName);
            EnsureUnityAssetFolder(scriptableObjectDirectory);
            SaveTreeViewToDirectory(treeView, scriptableObjectDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetDataRootReferences(data, scriptableObjectDirectory);
            SetCategoryDataReferencesRecursivly(scriptableObjectDirectory);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            CleanUnusedAssetsInDirectory(treeView, scriptableObjectDirectory);
        }

        private static void MoveDataAndChildDirectoryIfExists(MenuData data, string newDirectory, string newFileName)
        {
            string oldPath = AssetDatabase.GetAssetPath(data);
            if (string.IsNullOrEmpty(oldPath))
                return;

            string oldDirectory = Path.GetDirectoryName(oldPath);
            string oldFileName = Path.GetFileNameWithoutExtension(oldPath);

            if (oldDirectory.Equals(newDirectory) && oldFileName.Equals(newFileName))
                return;

            if (!AssetDatabase.IsValidFolder(NormalizeAssetPath(newDirectory)))
            {
                var parent = Path.GetDirectoryName(NormalizeAssetPath(newDirectory));
                var name = Path.GetFileName(NormalizeAssetPath(newDirectory));
                AssetDatabase.CreateFolder(parent, name);
            }

            string newPath = Path.Combine(newDirectory, newFileName + ".asset");
            string normalizedNewPath = NormalizeAssetPath(newPath);
            if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(normalizedNewPath) != null)
                AssetDatabase.DeleteAsset(normalizedNewPath);

            string error = AssetDatabase.MoveAsset(
                NormalizeAssetPath(oldPath),
                NormalizeAssetPath(newPath));
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to move asset to '{newPath}': {error}");
                return;
            }

            string oldScriptableObjectDirectory = Path.Combine(oldDirectory, oldFileName);
            string newScriptableObjectDirectory = Path.Combine(newDirectory, newFileName);
            if (AssetDatabase.IsValidFolder(NormalizeAssetPath(oldScriptableObjectDirectory)))
            {
                error = AssetDatabase.MoveAsset(
                    NormalizeAssetPath(oldScriptableObjectDirectory),
                    NormalizeAssetPath(newScriptableObjectDirectory));
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

            for (int i = 0; i < item.Children.Count; i++)
                SaveTreeViewDataRecursively(item.Children[i], Path.Combine(directory, GetUniqueName(item)));
        }

        private static void SaveScriptableObject<T>(T instance, string directory, string fileName) where T : ScriptableObject
        {
            if (instance == null || string.IsNullOrEmpty(directory))
                return;

            EnsureUnityAssetFolder(directory);

            string path = Path.Combine(directory, fileName + ".asset");
            string assetPath = AssetDatabase.GetAssetPath(instance);

            if (string.IsNullOrEmpty(assetPath))
                AssetDatabase.CreateAsset(instance, NormalizeAssetPath(path));
            else if (NormalizeAssetPath(assetPath) != NormalizeAssetPath(path))
            {
                string error = AssetDatabase.MoveAsset(
                    NormalizeAssetPath(assetPath),
                    NormalizeAssetPath(path));

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Failed to move asset from '{assetPath}' to '{path}': {error}");
            }
            else
            {
                EditorUtility.CopySerialized(instance, AssetDatabase.LoadAssetAtPath<T>(NormalizeAssetPath(path)));
                EditorUtility.SetDirty(instance);
            }
        }

        private static void SetDataRootReferences(MenuData data, string directory)
        {
            var assets = new List<ScriptableObject>();
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
                assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(NormalizeAssetPath(file)));

            data.Root = assets.ToArray();
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        private static void SetCategoryDataReferencesRecursivly(string directory)
        {
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
            {
                var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>(NormalizeAssetPath(file));
                if (data == null)
                    continue;

                string childDirectory = Path.Combine(directory, data.name);
                if (!Directory.Exists(childDirectory))
                    continue;

                // Get all child asset files, order by file name (which starts with index)
                var childFiles = Directory.GetFiles(childDirectory, "*.asset")
                                      .OrderBy(fileName => Path.GetFileName(fileName))
                                      .ToArray();

                var assets = new List<ScriptableObject>();
                foreach (var childFile in childFiles)
                    assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(NormalizeAssetPath(childFile)));

                data.GetType().GetField("Data")?.SetValue(data, assets.ToArray());
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();

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
                    AssetDatabase.DeleteAsset(NormalizeAssetPath(assetPath));

            foreach (var directoryPath in allDirectories.OrderByDescending(directory => directory.Length))
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
            string name = "Asset_";
            if (!string.IsNullOrEmpty(item.Name))
                name = $"{item.UniqueName.Replace(" ", "-")}_";

            if (item.Parent != null)
            {
                var index = item.Parent.Children.IndexOf(item);
                name = $"{index:D4}_{name}";
            }

            var itemData = item.UserData as ScriptableObject;
            if (itemData != null)
            {
                name += (itemData as MenuTypeDataBase).ID;
                itemData.name = name;
            }

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

            if (!AssetDatabase.IsValidFolder(NormalizeAssetPath(path)))
                return;

            AssetDatabase.DeleteAsset(NormalizeAssetPath(path));
        }

        private static string NormalizeAssetPath(string path) =>
            path.Replace('\\', '/');

        private static void EnsureUnityAssetFolder(string directory)
        {
            string normalized = NormalizeAssetPath(directory);
            if (!AssetDatabase.IsValidFolder(normalized))
            {
                string parent = Path.GetDirectoryName(normalized);
                string name = Path.GetFileName(normalized);
                if (!AssetDatabase.IsValidFolder(parent))
                    EnsureUnityAssetFolder(parent); // Recursively ensure parent exists
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
#endif