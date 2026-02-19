using Device_Library.Models.Data;
using Device_Library.Models.Data.Structs;
using Device_Library.Modules;
using Device_Library.ViewModels;
using System;
using System.Text;

namespace Device_Library.Views
{

    public class ConsoleUi
    {
        private readonly ConsoleViewModel vm = new();
        private readonly string[] Operations =
        {
            "List - get devices in lib",
            "Search - search devices from lib",
            "Add - add new device in lib",
            "Remove - remove device from lib",
            "Exit - exit from program"
        };
        public void InitUi()
        {
            vm.InitVM();
            Console.WriteLine($"Заprivateгружено устройств: {vm.DeviceList!.Count}\n");

            Console.Write("==== Device library ====\n" +
                "Enter \"help\" to get command list...\n");
            ConsoleCycle();
        }
        private void ConsoleCycle()
        {

            bool onExit = false;
            while (true)
            {
                Console.Write("DC >> ");
                string input = (Console.ReadLine() ?? "").Replace(" ", "");
                switch (input.ToLower())
                {
                    case "list":
                        ListDevices();
                        break;
                    case "search":
                        SearchDevice();
                        break;
                    case "add":
                        AddDevice();
                        break;
                    case "remove":
                        Remove();
                        break;
                    case "exit":
                        onExit = true;
                        break;
                    case "help":
                        commandList();
                        break;
                    default: 
                        Console.WriteLine("Unknown command! Enter \"help\" to get command list...\n"); 
                        break;
                }
                if (onExit)
                {
                    break;
                }
            }
        }
        private void commandList()
        {
            Console.WriteLine($"\n");
            foreach (var op in Operations)
                Console.WriteLine($"{op}");
            Console.WriteLine($"\n");
        }
        private void ListDevices()
        {
            int i = 0;
            foreach (var device in vm.DeviceList)
            {
                Console.WriteLine($"\n{DeviceFormat(device, vm.DeviceList.IndexOf(device))}");
                i++;
            }
            Console.WriteLine($"Total: {i} devices\n");
        }
        private void SearchDevice()
        {
            int i = 0;
            Console.Write("Enter your search: ");
            string input = Console.ReadLine()?.Trim() ?? "";
            List<DeviceParams> findedDevices = SearchService.SearchDeviceFromList(vm.DeviceList, input);

            if (findedDevices! == null || findedDevices.Count == 0)
            {
                Console.WriteLine("Unknown device");
                return;
            }

            foreach (DeviceParams device in findedDevices!)
            {
                Console.WriteLine($"\n{DeviceFormat(device, vm.DeviceList.IndexOf(device))}");
                i++;
            }
            Console.WriteLine($"{i} devices found");
        }
        private void AddDevice()
        {
            var newDevice = InputModule.CreateDeviceFromConsole();

            if (newDevice != null)
            {
                vm.AddDevice(newDevice);
                Console.WriteLine("\n[SUCCESS]: Device saved to database!");
            }
            else
            {
                Console.WriteLine("\n[CANCELLED]: Invalid input.");
            }
        }
        public void Remove()
        {
            try
            {
                Console.Write($"Enter device index: ");
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int index))
                {
                    Console.WriteLine("[ERROR]: Please enter a valid numerical index.");
                    return;
                }
                vm.RemoveDevice(index - 1);
                Console.WriteLine($"[SUCCESS]: Device at index {index} has been removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex}");
            }
        }

        private string DeviceFormat(DeviceParams device, int index)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Device index: {index + 1}");
            sb.AppendLine($"==== {device.DeviceInfo.Manufacturer} {device.DeviceInfo.Model} ====");
            sb.AppendLine($"RAM: {device.HardwareInfo.Ram} | ROM: {device.HardwareInfo.Rom} ");
            sb.AppendLine($"CPU: {device.HardwareInfo.Processor} | Charge Speed: {device.HardwareInfo.ChargeSpeed}W");
            sb.AppendLine($"Cameras: ");
            foreach (var CamPar in device.HardwareInfo.Cameras)
                sb.AppendLine($"\tType: {CamPar.CameraType}, Megapixels: {CamPar.Megapixels}");
            sb.AppendLine($"Display: {device.DisplayInfo.Type} | Resolution: {device.DisplayInfo.Resolution}p | Refresh Rate: {device.DisplayInfo.ScreenRefresh}hz");
            sb.AppendLine($"OS: {device.SoftwareInfo.OsName} | Version: {device.SoftwareInfo.Version}");
            return sb.ToString();
        }
    }
}