using Device_Library.Models.Data;
using Device_Library.Models.Data.Structs;

namespace Device_Library.Modules
{
    public static class InputModule
    {
        public static DeviceParams? CreateDeviceFromConsole()
        {
            try
            {
                Console.WriteLine("\n--- Create New Device ---");

                // Тип устройства
                Console.Write("Type (0 for Phone, 1 for Tablet): ");
                DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), Console.ReadLine());

                // Базовая информация
                Console.Write("Manufacturer: ");
                string man = Console.ReadLine() ?? "Unknown";
                Console.Write("Model: ");
                string mod = Console.ReadLine() ?? "Unknown";
                var devInfo = new DeviceInfo(man, mod);

                // Параметры дисплея
                Console.Write("Resolution (p): ");
                int res = int.Parse(Console.ReadLine());

                Console.Write("Panel Type (AMOLED, OLED, LCD, IPS): ");
                DisplayType dType = (DisplayType)Enum.Parse(typeof(DisplayType), Console.ReadLine());

                Console.Write("Refresh Rate (Hz): ");
                int hz = int.Parse(Console.ReadLine());
                var display = new DisplayInfo(res, dType, hz);

                // Параметры аппаратной части
                Console.Write("Processor Name: ");
                string cpu = Console.ReadLine() ?? "Unknown";
                Console.Write("RAM (GB): ");
                int ram = int.Parse(Console.ReadLine());
                Console.Write("Storage (GB): ");
                int rom = int.Parse(Console.ReadLine());
                Console.Write("Charging (W): ");
                int watt = int.Parse(Console.ReadLine());

                // Камеры
                List<Camera> cameras = new();
                Console.Write("Number of cameras: ");
                int camCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < camCount; i++)
                {
                    Console.Write($"Camera {i + 1} Type (Main, Selfie, etc.): ");
                    ECameraType cType = (ECameraType)Enum.Parse(typeof(ECameraType), Console.ReadLine());
                    Console.Write("Megapixels: ");
                    int mp = int.TryParse(Console.ReadLine(), out int m) ? m : 0;
                    cameras.Add(new Camera(cType, mp));
                }
                var hardware = new HardwareInfo(cpu, ram, rom, watt, cameras);

                // Параметры программ
                Console.Write("OS Name: ");
                string os = Console.ReadLine() ?? "Android";
                Console.Write("OS Version: ");
                string ver = Console.ReadLine() ?? "1.0";
                var software = new SoftwareInfo(os, ver);

                return new DeviceParams(type, devInfo, display, hardware, software);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Input Error]: {ex.Message}");
                return null;
            }
        }
    }
}