using Device_Library.Models.Data;

namespace Device_Library.ViewModels
{
    public class ConsoleViewModel
    {
        public List<DeviceParams> DeviceList { get; private set; } = new();

        public void InitVM()
        {
            DeviceList = Library.InitiLib();
        }

        public void AddDevice(DeviceParams device)
        {
            if (device == null) return;

            DeviceList.Add(device);
            Library.SaveLib(DeviceList);
        }
        public void RemoveDevice(int index)
        {
            if (index >= 0 && index < DeviceList.Count)
            {
                DeviceList.RemoveAt(index);

                Library.SaveLib(DeviceList);
            }
            else
            {
                throw new Exception($"Index {index} is out of range. Current count: {DeviceList.Count}");
            }
        }
    }
}