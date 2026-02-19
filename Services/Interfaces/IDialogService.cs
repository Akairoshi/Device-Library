using Device_Library_WPF.ViewModels;

public interface IDialogService
{
	void ShowDialogWindow(IDeviceActions actions, DeviceItemViewModel device);
}
