using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public interface DataProfileSerializerInterface { }

    public partial class DataProfileSerializer : MonoBehaviour
    {
        private static string _serializationFileName = "NewProfile";

        public static void SetSerializationFileName(string name) =>
            _serializationFileName = name;

        public static string GetSerializationFileName() =>
            _serializationFileName;

        public void SerializeData<T>(string path, T data) where T : DataProfileSerializerInterface
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public T DeserializeData<T>(string path) where T : DataProfileSerializerInterface
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string GetDataPath(params string[] subFolders)
        {
            var path = new List<string>() { Application.dataPath };
            path.AddRange(subFolders);

            string folderPath = Path.GetFullPath(Path.Combine(path.ToArray()));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        public string GetUniqueFileName(string folderPath, string fileName)
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