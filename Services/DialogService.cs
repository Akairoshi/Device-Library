using Device_Library_WPF.ViewModels;
using Device_Library_WPF.Views;

// Сервис открытия диалоговых окон (view создаётся здесь, vm не зависит от window)
public class DialogService : IDialogService
{
    public void ShowDialogWindow(IDeviceActions action, DeviceItemViewModel device)
    {
        var window = new InfoWindow();

        var vm = new InfoViewModel(action, device);

        window.DataContext = vm;
        window.ShowDialog();
    }

}
