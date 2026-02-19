using System.Text.Json.Serialization;

namespace Device_Library.Models.Data.Structs
{
    public record struct Camera
    (
        ECameraType CameraType,
        int Megapixels
    );
}
