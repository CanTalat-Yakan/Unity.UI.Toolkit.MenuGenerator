using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public partial class UIMenuDataProfileSerializer : MonoBehaviour
    {
        public static void SerializeData<T>(T data, string fileName, string localResourcePath = null) where T : UIMenuDataProfile
        {
            var directoryPath = GetDataPath("..", "Resources", localResourcePath);
            var filePath = Path.Combine(directoryPath, $"{fileName}.json");

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static bool DeserializeData<T>(out T data, string fileName, string localResourcePath = null) where T : UIMenuDataProfile
        {
            var directoryPath = GetDataPath("..", "Resources", localResourcePath);
            var filePath = Path.Combine(directoryPath, $"{fileName}.json");

            data = ScriptableObject.CreateInstance<T>();
            data.name = fileName + " AutoCreated";
            if (!File.Exists(filePath))
                return false;
            var json = File.ReadAllText(filePath);
            data = JsonConvert.DeserializeObject<T>(json);
            data.name = fileName + " AutoCreated";
            return true;
        }

        public static string GetDataPath(params string[] subFolders)
        {
            var path = new List<string>() { Application.dataPath };
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