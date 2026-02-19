using Device_Library_WPF.Commands;
namespace Device_Library_WPF.ViewModels;

public class InfoViewModel : ViewModelBase
{
	private readonly IDeviceActions _actions;
	public event Action? RequestClose;
	public DeviceItemViewModel Device { get; }

	public RelayCommand RemoveCommand { get; }

	public InfoViewModel(IDeviceActions actions, DeviceItemViewModel device)
	{
		_actions = actions;
		Device = device;

		RemoveCommand = new RelayCommand(() => 
		{
			_actions.Remove(device);
			//Вызов функции закрытия окна из view
			RequestClose?.Invoke();
		});
	}
}
