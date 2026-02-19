using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using Device_Library_WPF.Models;

// Описание полей ListBoxItem для ui
namespace Device_Library_WPF.ViewModels
{
	public class DeviceItemViewModel : ViewModelBase
	{
		// === Простые поля ===
		private int _id;
		public int Id
		{
			get => _id;
			set => Set(ref _id, value);
		}

		private string _deviceType;
		public string DeviceType
		{
			get => _deviceType;
			set => Set(ref _deviceType, value);
		}

		private string _manufacturer;
		public string Manufacturer
		{
			get => _manufacturer;
			set => Set(ref _manufacturer, value);
		}

		private string _model;
		public string Model
		{
			get => _model;
			set => Set(ref _model, value);
		}

		private string _cpu;
		public string Cpu
		{
			get => _cpu;
			set => Set(ref _cpu, value);
		}

		private string _screenType;
		public string ScreenType
		{
			get => _screenType;
			set => Set(ref _screenType, value);
		}

		private int _refreshRate;
		public int RefreshRate
		{
			get => _refreshRate;
			set => Set(ref _refreshRate, value);
		}
		private int _resolution;
		public int Resolution
		{
			get => _resolution;
			set => Set(ref _resolution, value);
		}

		private int _chargeSpeed;
		public int ChargeSpeed
		{
			get => _chargeSpeed;
			set => Set(ref _chargeSpeed, value);
		}

		private string _osName;
		public string OsName
		{
			get => _osName;
			set => Set(ref _osName, value);
		}

		private string _osVersion;
		public string OsVersion
		{
			get => _osVersion;
			set => Set(ref _osVersion, value);
		}

		// === Сложные поля ===
		// Массив конфигов с памятью (от 1 и более)
		public record MemoryConfig(int Ram, int Rom);
		private MemoryConfig[] _memory = Array.Empty<MemoryConfig>();
		public MemoryConfig[] Memory
		{
			get => _memory;
			set
			{
				if (Set(ref _memory, value ?? Array.Empty<MemoryConfig>()))
					OnPropertyChanged(nameof(MemoryText));
			}
		}
		public string MemoryText =>
			_memory.Length == 0
			? string.Empty
			: string.Join(", ", Memory.Select(m => $"{m.Ram}/{m.Rom}"));


		// Массив конфигов с камерами (от 1 и более)
		public record CameraConfig(string CameraType, int MegaPixels);
		private CameraConfig[] _cameras = Array.Empty<CameraConfig>();
		public CameraConfig[] Cameras
		{
			get => _cameras;
			set
			{
				if (Set(ref _cameras, value ?? Array.Empty<CameraConfig>()))
					OnPropertyChanged(nameof(CamerasText));
			}
		}
		public string CamerasText =>
			_cameras.Length == 0
				? string.Empty
				: string.Join(", ", _cameras.Select(c => $"{c.CameraType} {c.MegaPixels}mp"));

		// Изображение для карточки устройства (Если не найдена то будет null,
		// который в View будет заменен на плейсхолдер
		private string _imagePath;
		private ImageSource? _deviceImage;
		public ImageSource? DeviceImage => _deviceImage;

		public string ImagePath
		{
			get => _imagePath;
			set
			{
				if (Set(ref _imagePath, value))
				{
					_deviceImage = LoadImage(_imagePath);
					OnPropertyChanged(nameof(DeviceImage));
				}
			}
		}

		private static ImageSource LoadImage(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return null;

			var fullPath = Path.Combine(AppContext.BaseDirectory, path);

			if (!File.Exists(fullPath))
				return null;
			try
			{
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.EndInit();
				bmp.Freeze();
				return bmp;
			}
			catch
			{
				return null;
			}
		}


		public DeviceItemViewModel(Device item)
		{
			Id = item.Id;
			Manufacturer = item.DeviceInfo.Manufacturer;
			Model = item.DeviceInfo.Model;
			Memory = item.HardwareInfo.MemoryConfigs.Select(m => new MemoryConfig(m.Ram, m.Rom))
				.ToArray();
			Cpu = item.HardwareInfo.Processor;
			ScreenType = item.DisplayInfo.Type.ToString();
			Resolution = item.DisplayInfo.Resolution;
			RefreshRate = item.DisplayInfo.ScreenRefresh;
			ImagePath = item.DeviceInfo.ImagePath;
			DeviceType = item.Type.ToString();
			Cameras = item.HardwareInfo.Cameras?
				.Select(c => new CameraConfig(
					c.CameraType.ToString(),
					c.Megapixels))
				.ToArray()
				?? Array.Empty<CameraConfig>();
			ChargeSpeed = item.HardwareInfo.ChargeSpeed;
			OsName = item.SoftwareInfo.OsName;
			OsVersion = item.SoftwareInfo.Version;
		}
	}
}
