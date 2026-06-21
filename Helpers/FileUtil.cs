using System.Text.Json;
using System.IO;

namespace BuyMore.Helpers
{
    public static class FileUtil
    {
        //private static readonly string _path = @"C:\Users\usman.tijani\Documents\Coh2\BuyMore\Files";
        private static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        //private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Coh2", "BuyMore", "Files");
        public static List<T> ReadFromFile<T>(string fileName)
        {
            var filePath = Path.Combine(_path, fileName);

            if (!File.Exists(filePath))
            {
                //Directory.CreateDirectory(_path);
                //File.Create(filePath).Close();
                File.WriteAllText(filePath, "[]");
                return new List<T>();
            }

            var json = File.ReadAllText(filePath);

            if(string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }
            
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public static void SaveToFile<T>(List<T> data, string fileName)
        {
            var filePath = Path.Combine(_path, fileName);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}