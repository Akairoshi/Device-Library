using Device_Library_WPF.Commands;
using Device_Library_WPF.Models;
using Device_Library_WPF.Models.Structs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Device_Library_WPF.ViewModels
{
	public sealed class AddDeviceViewModel : ViewModelBase
	{
		private readonly IDeviceRepository _repo;

		public AddDeviceViewModel(IDeviceRepository repo)
		{
			_repo = repo;

			Type = DeviceType.Phone;
			DisplayType = DisplayType.AMOLED;
			OsName = "Android";
			OsVersion = "14";
			ScreenRefresh = 120;

			AddCameraCommand = new RelayCommand(AddCamera);
			RemoveCameraCommand = new RelayCommand<CameraRowVM>(RemoveCamera);

			AddMemoryCommand = new RelayCommand(AddMemory);
			RemoveMemoryCommand = new RelayCommand<MemoryRowVM>(RemoveMemory);

			SaveCommand = new RelayCommand(Save);
			CancelCommand = new RelayCommand(() => RequestClose?.Invoke(false));

			BrowseImageCommand = new RelayCommand(BrowseImage);

			// стартовые строки
			Memories.Add(new MemoryRowVM { Ram = 8, Rom = 128, IsSelected = true });
			Cameras.Add(new CameraRowVM { Type = ECameraType.Main, Megapixels = 50 });
		}

		public event Action<bool>? RequestClose;

		public Array DeviceTypes => Enum.GetValues(typeof(DeviceType));
		public Array DisplayTypes => Enum.GetValues(typeof(DisplayType));
		public Array CameraTypes => Enum.GetValues(typeof(ECameraType));

		private DeviceType _type;
		public DeviceType Type
		{
			get => _type;
			set { _type = value; OnPropertyChanged(); }
		}

		private string _manufacturer = "";
		public string Manufacturer
		{
			get => _manufacturer;
			set { _manufacturer = value; OnPropertyChanged(); }
		}

		private string _model = "";
		public string Model
		{
			get => _model;
			set { _model = value; OnPropertyChanged(); }
		}

		private string _imagePath = "";
		public string ImagePath
		{
			get => _imagePath;
			set
			{
				_imagePath = value;
				OnPropertyChanged();
				UpdatePreviewImage();
			}
		}

		private int _resolution;
		public int Resolution
		{
			get => _resolution;
			set { _resolution = value; OnPropertyChanged(); }
		}

		private DisplayType _displayType;
		public DisplayType DisplayType
		{
			get => _displayType;
			set { _displayType = value; OnPropertyChanged(); }
		}

		private int _screenRefresh;
		public int ScreenRefresh
		{
			get => _screenRefresh;
			set { _screenRefresh = value; OnPropertyChanged(); }
		}

		private string _processor = "";
		public string Processor
		{
			get => _processor;
			set { _processor = value; OnPropertyChanged(); }
		}

		private int _chargeSpeed;
		public int ChargeSpeed
		{
			get => _chargeSpeed;
			set { _chargeSpeed = value; OnPropertyChanged(); }
		}

		private string _osName = "";
		public string OsName
		{
			get => _osName;
			set { _osName = value; OnPropertyChanged(); }
		}

		private string _osVersion = "";
		public string OsVersion
		{
			get => _osVersion;
			set { _osVersion = value; OnPropertyChanged(); }
		}

		public ObservableCollection<MemoryRowVM> Memories { get; } = new();
		public ObservableCollection<CameraRowVM> Cameras { get; } = new();

		private ImageSource? _deviceImage;
		public ImageSource? DeviceImage
		{
			get => _deviceImage;
			private set { _deviceImage = value; OnPropertyChanged(); }
		}

		public RelayCommand BrowseImageCommand { get; }
		public RelayCommand AddCameraCommand { get; }
		public RelayCommand<CameraRowVM> RemoveCameraCommand { get; }

		public RelayCommand AddMemoryCommand { get; }
		public RelayCommand<MemoryRowVM> RemoveMemoryCommand { get; }

		public RelayCommand SaveCommand { get; }
		public RelayCommand CancelCommand { get; }

		private void AddCamera()
			=> Cameras.Add(new CameraRowVM { Type = ECameraType.Main, Megapixels = 12 });

		private void RemoveCamera(CameraRowVM? cam)
		{
			if (cam == null) return;
			Cameras.Remove(cam);
		}

		private void AddMemory()
		{
			Memories.Add(new MemoryRowVM { Ram = 8, Rom = 128, IsSelected = Memories.Count == 0 });
		}

		private void RemoveMemory(MemoryRowVM? mem)
		{
			if (mem == null) return;

			var wasSelected = mem.IsSelected;
			Memories.Remove(mem);

			// если удалили выбранный — выберем первый оставшийся
			if (wasSelected && Memories.Count > 0)
				Memories[0].IsSelected = true;
		}

		private void BrowseImage()
		{
			var dialog = new OpenFileDialog
			{
				Title = "Select device image",
				Filter = "Images (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp",
				CheckFileExists = true,
				Multiselect = false
			};

			if (dialog.ShowDialog() != true)
				return;

			// Куда копируем
			var imagesDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Images");
			Directory.CreateDirectory(imagesDir);

			var srcPath = dialog.FileName;
			var ext = Path.GetExtension(srcPath);

			// Безопасное имя
			var baseName = Path.GetFileNameWithoutExtension(srcPath);
			baseName = string.Concat(baseName.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)));

			var fileName = $"{baseName}{ext}";
			var dstPath = Path.Combine(imagesDir, fileName);

			// Если файл уже есть - добавим суффикс
			int i = 1;
			while (File.Exists(dstPath))
			{
				fileName = $"{baseName}_{i}{ext}";
				dstPath = Path.Combine(imagesDir, fileName);
				i++;
			}

			File.Copy(srcPath, dstPath, overwrite: false);

			// Храним относительный путь
			ImagePath = Path.Combine("Assets", "Images", fileName).Replace('\\', '/');
		}

		private async void Save()
		{
			if (string.IsNullOrWhiteSpace(Manufacturer) || string.IsNullOrWhiteSpace(Model))
				return;

			// --- собрать ВСЕ memory configs ---
			var memList = Memories
				.Where(m => m.Ram > 0 && m.Rom > 0)
				.Select(m => new MemoryConfig(m.Ram, m.Rom))
				.ToList();

			if (memList.Count == 0)
				return;

			var primary = memList[0];

			// собрать камеры
			var cameraList = Cameras
				.Where(c => c.Megapixels > 0)
				.Select(c => new Camera(c.Type, c.Megapixels))
				.ToList();

			var device = new Device(
				Id: 0,
				Type: Type,
				DeviceInfo: new DeviceInfo(Manufacturer, Model, ImagePath),
				DisplayInfo: new DisplayInfo(Resolution, DisplayType, ScreenRefresh),
				HardwareInfo: new HardwareInfo(
					Processor,
					primary.Ram, primary.Rom,
					memList,
					ChargeSpeed,
					cameraList
				),
				SoftwareInfo: new SoftwareInfo(OsName, OsVersion)
			);

			await _repo.AddAsync(device);
			RequestClose?.Invoke(true);
		}

		private void UpdatePreviewImage()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(ImagePath))
				{
					DeviceImage = null;
					return;
				}

				var path = ImagePath;
				if (!Path.IsPathRooted(path))
					path = Path.Combine(AppContext.BaseDirectory, path);

				if (!File.Exists(path))
				{
					DeviceImage = null;
					return;
				}

				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.UriSource = new Uri(path, UriKind.Absolute);
				bmp.EndInit();
				bmp.Freeze();

				DeviceImage = bmp;
			}
			catch
			{
				DeviceImage = null;
			}
		}
	}

	public sealed class CameraRowVM : ViewModelBase
	{
		private ECameraType _type;
		public ECameraType Type
		{
			get => _type;
			set { _type = value; OnPropertyChanged(); }
		}

		private int _megapixels;
		public int Megapixels
		{
			get => _megapixels;
			set { _megapixels = value; OnPropertyChanged(); }
		}
	}

	public sealed class MemoryRowVM : ViewModelBase
	{
		private int _ram;
		public int Ram
		{
			get => _ram;
			set { _ram = value; OnPropertyChanged(); }
		}

		private int _rom;
		public int Rom
		{
			get => _rom;
			set { _rom = value; OnPropertyChanged(); }
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set { _isSelected = value; OnPropertyChanged(); }
		}
	}
}
