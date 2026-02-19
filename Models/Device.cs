using Device_Library_WPF.Models.Structs;

namespace Device_Library_WPF.Models
{
    //Структура данных для устройств
    public record Device
    (
        int Id,
        DeviceType Type,
        DeviceInfo DeviceInfo,
        DisplayInfo DisplayInfo,
        HardwareInfo HardwareInfo,
        SoftwareInfo SoftwareInfo
    );
}