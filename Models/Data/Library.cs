using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Device_Library.Models.Data
{
    public static class Library
    {
        private static readonly string _filesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "DataLib");
        private static readonly string _fileName = "DataLib.json";
        private static string _filePath = Path.Combine(_filesDir, _fileName);
        public static List<DeviceParams> InitiLib()
        {
            Console.WriteLine(_filePath);
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found!");
                return [];
            }

            string jsonString = File.ReadAllText(_filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            options.Converters.Add(new JsonStringEnumConverter());

            try
            {
                return JsonSerializer.Deserialize<List<DeviceParams>>(jsonString, options) ?? new List<DeviceParams>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON error: {ex.Message}");
                return [];
            }
        }
        public static void SaveLib(List<DeviceParams> deviceList)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                options.Converters.Add(new JsonStringEnumConverter());

                string jsonString = JsonSerializer.Serialize(deviceList, options);


                if (!Directory.Exists(_filesDir))
                {
                    Directory.CreateDirectory(_filesDir);
                }

                File.WriteAllText(_filePath, jsonString);
                Console.WriteLine("Data saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }
    }
}
