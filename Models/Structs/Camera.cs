using System.Text.Json.Serialization;

namespace Device_Library_WPF.Models.Structs
{
    public record struct Camera
    (
        ECameraType CameraType,
        int Megapixels
    );
}
