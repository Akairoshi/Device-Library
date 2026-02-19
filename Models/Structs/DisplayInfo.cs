namespace Device_Library_WPF.Models.Structs
{
    public record struct DisplayInfo(
        int Resolution,
        DisplayType Type,
        int ScreenRefresh
    );
}