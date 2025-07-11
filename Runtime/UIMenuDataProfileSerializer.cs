using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public partial class UIMenuDataProfileSerializer : MonoBehaviour
    {
        public static void SerializeData<T>(
            T data,
            string fileName,
            bool useParentDirectory = true,
            string localResourcePath = null)
        {
            var directoryPath = GetDataPath(useParentDirectory, "Resources", localResourcePath);
            var filePath = Path.Combine(directoryPath, $"{fileName}.json");

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new UnityColorJsonConverter());
            settings.ContractResolver = new IgnoreUnityObjectContractResolver();

            var json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
            File.WriteAllText(filePath, json);
        }

        public static bool DeserializeData<T>(
            out T data,
            string fileName,
            bool useParentDirectory = true,
            string localResourcePath = null)
        {
            data = default;

            var directoryPath = GetDataPath(useParentDirectory, "Resources", localResourcePath);
            var filePath = Path.Combine(directoryPath, $"{fileName}.json");

            if (!File.Exists(filePath))
                return false;

            var json = File.ReadAllText(filePath);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new UnityColorJsonConverter());
            settings.ContractResolver = new IgnoreUnityObjectContractResolver();

            data = JsonConvert.DeserializeObject<T>(json, settings);
            return data != null;
        }

        public static string GetDataPath(bool useParentDirectory, params string[] subFolders)
        {
            var path = new List<string>() { Application.dataPath };

            if (useParentDirectory)
                path.Add("..");

            foreach (var folder in subFolders)
                if (!string.IsNullOrEmpty(folder))
                    path.Add(folder);

            string folderPath = Path.GetFullPath(Path.Combine(path.ToArray()));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        public static string GetUniqueFileName(string folderPath, string fileName)
        {
            string fullPath = Path.Combine(folderPath, fileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            int counter = 1;

            while (File.Exists(fullPath))
            {
                fullPath = Path.Combine(folderPath, $"{fileNameWithoutExt} ({counter}){extension}");
                counter++;
            }

            return Path.GetFileName(fullPath);
        }
    }
}