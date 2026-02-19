using Device_Library_WPF.Models;
using Device_Library_WPF.Models.Structs;

namespace Device_Library_WPF.Data;

public static class Seed
{
	public static IReadOnlyList<Device> Devices => new List<Device>
	{
		new Device(
			Id: 1,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel", @"Assets/Images/pixel1.png"),
			DisplayInfo: new DisplayInfo(1920, DisplayType.AMOLED, 60),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 821",
				4, 32,
				new List<MemoryConfig>
				{
					new MemoryConfig(4, 32),
					new MemoryConfig(4, 128)
				},
				18,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 12),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "10")
		),

		new Device(
			Id: 2,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 2", @"Assets/Images/pixel2.png"),
			DisplayInfo: new DisplayInfo(1920, DisplayType.AMOLED, 60),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 835",
				4, 64,
				new List<MemoryConfig>
				{
					new MemoryConfig(4, 64),
					new MemoryConfig(4, 128)
				},
				18,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 12),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "11")
		),

		new Device(
			Id: 3,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 3", @"Assets/Images/pixel3.png"),
			DisplayInfo: new DisplayInfo(2160, DisplayType.OLED, 60),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 845",
				4, 64,
				new List<MemoryConfig>
				{
					new MemoryConfig(4, 64),
					new MemoryConfig(4, 128)
				},
				18,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 12),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "12")
		),

		new Device(
			Id: 4,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 4", @"Assets/Images/pixel4.png"),
			DisplayInfo: new DisplayInfo(2280, DisplayType.OLED, 90),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 855",
				6, 64,
				new List<MemoryConfig>
				{
					new MemoryConfig(6, 64),
					new MemoryConfig(6, 128)
				},
				18,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 12),
					new Camera(ECameraType.Telephoto, 16),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "13")
		),

		new Device(
			Id: 5,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 5", @"Assets/Images/pixel5.png"),
			DisplayInfo: new DisplayInfo(2340, DisplayType.OLED, 90),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 765G",
				8, 128,
				new List<MemoryConfig>
				{
					new MemoryConfig(8, 128)
				},
				18,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 12),
					new Camera(ECameraType.Ultrawide, 16),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "14")
		),

		new Device(
			Id: 6,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 6", @"Assets/Images/pixel6.png"),
			DisplayInfo: new DisplayInfo(2400, DisplayType.AMOLED, 90),
			HardwareInfo: new HardwareInfo(
				"Google Tensor",
				8, 128,
				new List<MemoryConfig>
				{
					new MemoryConfig(8, 128),
					new MemoryConfig(8, 256)
				},
				30,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 50),
					new Camera(ECameraType.Ultrawide, 12),
					new Camera(ECameraType.Selfie, 8)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "14")
		),

		new Device(
			Id: 7,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 7", @"Assets/Images/pixel7.png"),
			DisplayInfo: new DisplayInfo(2400, DisplayType.AMOLED, 90),
			HardwareInfo: new HardwareInfo(
				"Google Tensor G2",
				8, 128,
				new List<MemoryConfig>
				{
					new MemoryConfig(8, 128),
					new MemoryConfig(8, 256)
				},
				30,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 50),
					new Camera(ECameraType.Ultrawide, 12),
					new Camera(ECameraType.Selfie, 10)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "14")
		),

		new Device(
			Id: 8,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Google", "Pixel 8", @"Assets/Images/pixel8.png"),
			DisplayInfo: new DisplayInfo(2400, DisplayType.OLED, 120),
			HardwareInfo: new HardwareInfo(
				"Google Tensor G3",
				8, 128,
				new List<MemoryConfig>
				{
					new MemoryConfig(8, 128),
					new MemoryConfig(8, 256)
				},
				27,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 50),
					new Camera(ECameraType.Ultrawide, 12),
					new Camera(ECameraType.Selfie, 10)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "14")
		),
		//Nothing
		new Device(
			Id: 9,
			Type: DeviceType.Phone,
			DeviceInfo: new DeviceInfo("Nothing", "Phone (1)", @"Assets/Images/nothing1.png"),
			DisplayInfo: new DisplayInfo(2400, DisplayType.OLED, 120),
			HardwareInfo: new HardwareInfo(
				"Snapdragon 778G+",
				8, 128,
				new List<MemoryConfig>
				{
					new MemoryConfig(8, 128),
					new MemoryConfig(8, 256),
					new MemoryConfig(12, 256)
				},
				33,
				new List<Camera>
				{
					new Camera(ECameraType.Main, 50),
					new Camera(ECameraType.Ultrawide, 50),
					new Camera(ECameraType.Selfie, 16)
				}
			),
			SoftwareInfo: new SoftwareInfo("Android", "14")
		),

			new Device(
				Id: 10,
				Type: DeviceType.Phone,
				DeviceInfo: new DeviceInfo("Nothing", "Phone (2)", @"Assets/Images/nothing2.png"),
				DisplayInfo: new DisplayInfo(2412, DisplayType.OLED, 120),
				HardwareInfo: new HardwareInfo(
					"Snapdragon 8+ Gen 1",
					8, 128,
					new List<MemoryConfig>
					{
						new MemoryConfig(8, 128),
						new MemoryConfig(12, 256),
						new MemoryConfig(12, 512)
					},
					45,
					new List<Camera>
					{
						new Camera(ECameraType.Main, 50),
						new Camera(ECameraType.Ultrawide, 50),
						new Camera(ECameraType.Selfie, 32)
					}
				),
				SoftwareInfo: new SoftwareInfo("Android", "14")
			),

			new Device(
				Id: 11,
				Type: DeviceType.Phone,
				DeviceInfo: new DeviceInfo("Nothing", "Phone (2a)", @"Assets/Images/nothing2a.png"),
				DisplayInfo: new DisplayInfo(2412, DisplayType.AMOLED, 120),
				HardwareInfo: new HardwareInfo(
					"Dimensity 7200 Pro",
					8, 128,
					new List<MemoryConfig>
					{
						new MemoryConfig(8, 128),
						new MemoryConfig(12, 256)
					},
					45,
					new List<Camera>
					{
						new Camera(ECameraType.Main, 50),
						new Camera(ECameraType.Ultrawide, 50),
						new Camera(ECameraType.Selfie, 32)
					}
				),
				SoftwareInfo: new SoftwareInfo("Android", "14")
			)

	};
}
