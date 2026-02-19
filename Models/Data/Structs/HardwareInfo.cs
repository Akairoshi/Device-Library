namespace Device_Library.Models.Data.Structs
{
    public record struct HardwareInfo
    (
        string Processor,
        int Ram, int Rom,
        int ChargeSpeed,
        List<Camera> Cameras
    );
}
