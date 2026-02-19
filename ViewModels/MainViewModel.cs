using Device_Library_WPF.Commands;
using Device_Library_WPF.Data;
using Device_Library_WPF.Views;
using System.Collections.ObjectModel;
using System.IO;

namespace Device_Library_WPF.ViewModels
{
	public class MainViewModel : ViewModelBase, IDeviceActions
	{
		private readonly IDialogService _dialogService;
		private readonly IDeviceRepository _repository;

		private string _searchText = "";

		public ObservableCollection<DeviceItemViewModel> Devices { get; } = new();

		// Бинды для кнопок
		public RelayCommand<DeviceItemViewModel> RemoveCommand { get; }
		public RelayCommand<DeviceItemViewModel> InfoCommand { get; }
		public RelayCommand SearchCommand { get; }
		public RelayCommand AddDeviceCommand { get; }
		public string SearchText
		{
			get => _searchText;
			set => Set(ref _searchText, value);
		}


		public async Task LoadAsync()
		{
			var list = await _repository.GetAllAsync();

			Devices.Clear();
			foreach (var d in list)
				Devices.Add(new DeviceItemViewModel(d));
		}

		private async void ApplySearch()
		{
			var list = await _repository.SearchAsync(SearchText);

			Devices.Clear();
			foreach (var d in list)
				Devices.Add(new DeviceItemViewModel(d));
		}

		public async void Remove(DeviceItemViewModel item)
		{
			if (item == null) return;

			await _repository.DeleteAsync(item.Id);
			Devices.Remove(item);
		}

		public async void AddDevice()
		{
			var vm = new AddDeviceViewModel(_repository);
			var w = new AddWindow(vm);

			// Перезагружаем список после добавления девайса
			if (w.ShowDialog() == true)
				await LoadAsync();
		}

		public void ShowInfo(DeviceItemViewModel device)
		{
			var window = new InfoWindow();
			window.DataContext = new InfoViewModel(this, device);

			window.ShowDialog();
		}

		// Иниуиализация зависимостей и команд ViewModel
		// repository работает с sqlite, dialogService отвечает за ui-диалоги
		public MainViewModel()
		{
			var dbPath = Path.Combine(AppContext.BaseDirectory, "devices.db");
			_repository = new SqliteDeviceRepository(dbPath);

			_dialogService = new DialogService();

			InfoCommand = new RelayCommand<DeviceItemViewModel>(ShowInfo);
			AddDeviceCommand = new RelayCommand(AddDevice);
			SearchCommand = new RelayCommand(ApplySearch);
		}
	}

}