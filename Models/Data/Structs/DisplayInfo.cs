namespace Device_Library.Models.Data.Structs
{
    public record struct DisplayInfo(
        int Resolution,
        DisplayType Type,
        int ScreenRefresh
    );
}