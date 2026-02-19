using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UDPMonitor.Core.Serialization
{
    public static class JsonConversion
    {
        public static T Deserialize<T>(string jsonString)
        {
            return TryDeserialize(jsonString, out T result) ? result : default(T);
        }

        public static T Deserialize<T>(byte[] jsonDataBytes)
        {
            string jsonString = System.Text.Encoding.UTF8.GetString(jsonDataBytes);

            return TryDeserialize(jsonString, out T result) ? result : default(T);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (JsonException)
            {
                result = default;
                return false;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        public static T ReadFromFile<T>(string filePath, bool deleteAfterRead = false)
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);

                if (deleteAfterRead)
                {
                    File.Delete(filePath);
                }

                return Deserialize<T>(jsonString);
            }

            return default;
        }

        public static void WriteToFile<T>(T item, string filePath, bool isIndented = true)
        {
            string jsonString = Serialize(item, isIndented);

            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, jsonString);
        }

        public static string Serialize<T>(T item, bool isIndented = true)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = isIndented,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(item, options);
        }

        public static byte[] SerializeToBytes<T>(T item)
        {
            // Serializza l'oggetto nell'array di byte
            string jsonString = JsonSerializer.Serialize(item);
            return System.Text.Encoding.UTF8.GetBytes(jsonString);
        }

        public static List<T> ReadFromFolder<T>(string directory, bool deleteAfterRead = false)
        {
            var directoryInfo = new DirectoryInfo(directory);

            var files = directoryInfo.GetFiles().ToList();

            List<T> items = new List<T>();

            files.ForEach(file =>
            {
                items.Add(JsonConversion.ReadFromFile<T>(file.FullName, deleteAfterRead));
            });

            return items;
        }
    }
}