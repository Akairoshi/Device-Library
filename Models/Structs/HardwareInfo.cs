namespace Device_Library_WPF.Models.Structs
{
    public record struct HardwareInfo
    (
        string Processor,
        int Ram, int Rom,
		List<MemoryConfig> MemoryConfigs,
        int ChargeSpeed,
        List<Camera> Cameras
    );
}
