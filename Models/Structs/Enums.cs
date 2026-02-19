using System.Text.Json.Serialization;

namespace Device_Library_WPF.Models.Structs
{
    public enum DeviceType { Phone, Tablet };

    public enum DisplayType
    {
        Unknown,
        AMOLED,
        OLED,
        LCD,
        IPS,
        TFT,
        [JsonPropertyName("Dynamic AMOLED 2X")]
        DynamicAMOLED2X
    }

    public enum ECameraType
    {
        Main,
        Wide,
        Ultrawide,  
        Telephoto,
        Macro,
        Periscope,
        Depth,
        Selfie
    }
}