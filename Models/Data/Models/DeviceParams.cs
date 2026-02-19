using Device_Library.Models.Data.Structs;

namespace Device_Library.Models.Data
{
    //Структура данных для устройств
    public record DeviceParams
    (
        DeviceType Type,
        DeviceInfo DeviceInfo,
        DisplayInfo DisplayInfo,
        HardwareInfo HardwareInfo,
        SoftwareInfo SoftwareInfo
    );
}