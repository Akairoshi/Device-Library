using System.IO;
using Microsoft.Data.Sqlite;
using Device_Library_WPF.Models;
using Device_Library_WPF.Models.Structs;

// Работа с sqlite дб

namespace Device_Library_WPF.Data
{
	public sealed class SqliteDeviceRepository : IDeviceRepository
	{
		private readonly string _cs;

		// Создание дб и добавление туда стандартного списка устройств
		public SqliteDeviceRepository(string dbPath)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
			_cs = $"Data Source={dbPath}";
			Init();
			SeedIfEmpty();
		}

		// Создание дб
		private void Init()
		{
			using var con = new SqliteConnection(_cs);
			con.Open();

			using var cmd = con.CreateCommand();
			cmd.CommandText = @"
				PRAGMA foreign_keys = ON;

				CREATE TABLE IF NOT EXISTS Devices (
					Id INTEGER PRIMARY KEY AUTOINCREMENT,
					Type INTEGER NOT NULL,
					Manufacturer TEXT NOT NULL,
					Model TEXT NOT NULL,
					ImagePath TEXT,
					Resolution INTEGER NOT NULL,
					DisplayType INTEGER NOT NULL,
					ScreenRefresh INTEGER NOT NULL,
					Processor TEXT NOT NULL,

					-- primary/default memory config for compatibility
					Ram INTEGER NOT NULL,
					Rom INTEGER NOT NULL,

					ChargeSpeed INTEGER NOT NULL,
					OsName TEXT NOT NULL,
					OsVersion TEXT NOT NULL
				);

				CREATE TABLE IF NOT EXISTS Cameras (
					Id INTEGER PRIMARY KEY AUTOINCREMENT,
					DeviceId INTEGER NOT NULL,
					CameraType INTEGER NOT NULL,
					Megapixels INTEGER NOT NULL,
					FOREIGN KEY(DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
				);
				CREATE INDEX IF NOT EXISTS IX_Cameras_DeviceId ON Cameras(DeviceId);

				CREATE TABLE IF NOT EXISTS MemoryConfigs (
					Id INTEGER PRIMARY KEY AUTOINCREMENT,
					DeviceId INTEGER NOT NULL,
					Ram INTEGER NOT NULL,
					Rom INTEGER NOT NULL,
					FOREIGN KEY(DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
				);
				CREATE INDEX IF NOT EXISTS IX_MemoryConfigs_DeviceId ON MemoryConfigs(DeviceId);
				";
			cmd.ExecuteNonQuery();
		}

		// Получение всех устройств из дб
		public List<Device> GetAll()
		{
			using var con = new SqliteConnection(_cs);
			con.Open();

			var devices = new List<Device>();

			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = @"
					SELECT
						Id, Type, Manufacturer, Model, ImagePath,
						Resolution, DisplayType, ScreenRefresh,
						Processor, Ram, Rom, ChargeSpeed,
						OsName, OsVersion
					FROM Devices;
					";
				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var id = r.GetInt32(0);

					var d = new Device(
						Id: id,
						Type: (DeviceType)r.GetInt32(1),
						DeviceInfo: new DeviceInfo(
							r.GetString(2),
							r.GetString(3),
							r.IsDBNull(4) ? "" : r.GetString(4)
						),
						DisplayInfo: new DisplayInfo(
							r.GetInt32(5),
							(DisplayType)r.GetInt32(6),
							r.GetInt32(7)
						),
						HardwareInfo: new HardwareInfo(
							Processor: r.GetString(8),
							Ram: r.GetInt32(9),
							Rom: r.GetInt32(10),
							ChargeSpeed: r.GetInt32(11),
							Cameras: new List<Camera>(),
							MemoryConfigs: new List<MemoryConfig>()
						),
						SoftwareInfo: new SoftwareInfo(
							r.GetString(12),
							r.GetString(13)
						)
					);

					devices.Add(d);
				}
			}

			if (devices.Count == 0) return devices;

			// Получение всех конфигов камер из дб
			var cams = new Dictionary<int, List<Camera>>();
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT DeviceId, CameraType, Megapixels FROM Cameras;";
				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var deviceId = r.GetInt32(0); // Получение id устройства, для которого получаем конфиги камеры 
					if (!cams.TryGetValue(deviceId, out var list))
						cams[deviceId] = list = new List<Camera>();

					list.Add(new Camera((ECameraType)r.GetInt32(1), r.GetInt32(2)));
				}
			}

			// Получение всех конфигов памяти из дб
			var mems = new Dictionary<int, List<MemoryConfig>>();
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT DeviceId, Ram, Rom FROM MemoryConfigs;";
				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var deviceId = r.GetInt32(0); // Получение id устройства, для которого получаем конфиги памяти
					if (!mems.TryGetValue(deviceId, out var list))
						mems[deviceId] = list = new List<MemoryConfig>();

					list.Add(new MemoryConfig(r.GetInt32(1), r.GetInt32(2)));
				}
			}

			for (int i = 0; i < devices.Count; i++)
			{
				var d = devices[i];

				var camList = cams.TryGetValue(d.Id, out var cl) ? cl : new List<Camera>();
				var memList = mems.TryGetValue(d.Id, out var ml) ? ml : new List<MemoryConfig>();

				if (memList.Count == 0)
					memList.Add(new MemoryConfig(d.HardwareInfo.Ram, d.HardwareInfo.Rom));

				var primary = memList[0];

				var hw = new HardwareInfo(
					Processor: d.HardwareInfo.Processor,
					Ram: primary.Ram,
					Rom: primary.Rom,
					ChargeSpeed: d.HardwareInfo.ChargeSpeed,
					Cameras: camList,
					MemoryConfigs: memList
				);

				devices[i] = new Device(d.Id, d.Type, d.DeviceInfo, d.DisplayInfo, hw, d.SoftwareInfo);
			}

			return devices;
		}

		public int Add(Device device)
		{
			using var con = new SqliteConnection(_cs);
			con.Open();
			using var tx = con.BeginTransaction();

			var memList = device.HardwareInfo.MemoryConfigs;
			if (memList == null || memList.Count == 0)
				memList = new List<MemoryConfig> { new MemoryConfig(device.HardwareInfo.Ram, device.HardwareInfo.Rom) };
			var primary = memList[0];

			int id;
			using (var cmd = con.CreateCommand())
			{
				cmd.Transaction = tx;
				cmd.CommandText = @"
					INSERT INTO Devices (
						Type, Manufacturer, Model, ImagePath,
						Resolution, DisplayType, ScreenRefresh,
						Processor, Ram, Rom, ChargeSpeed,
						OsName, OsVersion
					) VALUES (
						@Type, @Man, @Model, @Img,
						@Res, @Disp, @Ref,
						@Cpu, @Ram, @Rom, @Chg,
						@Os, @Ver
					);
					SELECT last_insert_rowid();
					";
				cmd.Parameters.AddWithValue("@Type", (int)device.Type);
				cmd.Parameters.AddWithValue("@Man", device.DeviceInfo.Manufacturer);
				cmd.Parameters.AddWithValue("@Model", device.DeviceInfo.Model);

				cmd.Parameters.AddWithValue("@Img",
					string.IsNullOrWhiteSpace(device.DeviceInfo.ImagePath)
						? DBNull.Value
						: device.DeviceInfo.ImagePath);

				cmd.Parameters.AddWithValue("@Res", device.DisplayInfo.Resolution);
				cmd.Parameters.AddWithValue("@Disp", (int)device.DisplayInfo.Type);
				cmd.Parameters.AddWithValue("@Ref", device.DisplayInfo.ScreenRefresh);

				cmd.Parameters.AddWithValue("@Cpu", device.HardwareInfo.Processor);
				cmd.Parameters.AddWithValue("@Ram", primary.Ram);
				cmd.Parameters.AddWithValue("@Rom", primary.Rom);
				cmd.Parameters.AddWithValue("@Chg", device.HardwareInfo.ChargeSpeed);

				cmd.Parameters.AddWithValue("@Os", device.SoftwareInfo.OsName);
				cmd.Parameters.AddWithValue("@Ver", device.SoftwareInfo.Version);

				id = Convert.ToInt32(cmd.ExecuteScalar());
			}
			if (device.HardwareInfo.Cameras is { Count: > 0 })
			{
				using var cmd = con.CreateCommand();
				cmd.Transaction = tx;
				cmd.CommandText = "INSERT INTO Cameras(DeviceId, CameraType, Megapixels) VALUES (@Id,@T,@Mp);";
				var pId = cmd.Parameters.Add("@Id", SqliteType.Integer);
				var pT = cmd.Parameters.Add("@T", SqliteType.Integer);
				var pMp = cmd.Parameters.Add("@Mp", SqliteType.Integer);

				foreach (var c in device.HardwareInfo.Cameras)
				{
					pId.Value = id;
					pT.Value = (int)c.CameraType;
					pMp.Value = c.Megapixels;
					cmd.ExecuteNonQuery();
				}
			}

			{
				using var cmd = con.CreateCommand();
				cmd.Transaction = tx;
				cmd.CommandText = "INSERT INTO MemoryConfigs(DeviceId, Ram, Rom) VALUES (@Id,@Ram,@Rom);";
				var pId = cmd.Parameters.Add("@Id", SqliteType.Integer);
				var pRam = cmd.Parameters.Add("@Ram", SqliteType.Integer);
				var pRom = cmd.Parameters.Add("@Rom", SqliteType.Integer);

				foreach (var m in memList)
				{
					pId.Value = id;
					pRam.Value = m.Ram;
					pRom.Value = m.Rom;
					cmd.ExecuteNonQuery();
				}
			}

			tx.Commit();
			return id;
		}

		public void Delete(int id)
		{
			using var con = new SqliteConnection(_cs);
			con.Open();
			using var cmd = con.CreateCommand();
			cmd.CommandText = "DELETE FROM Devices WHERE Id=@Id;";
			cmd.Parameters.AddWithValue("@Id", id);
			cmd.ExecuteNonQuery();
		}

		public void Update(int id, Device device)
		{
			using var con = new SqliteConnection(_cs);
			con.Open();
			using var tx = con.BeginTransaction();

			var memList = device.HardwareInfo.MemoryConfigs;
			if (memList == null || memList.Count == 0)
				memList = new List<MemoryConfig> { new MemoryConfig(device.HardwareInfo.Ram, device.HardwareInfo.Rom) };
			var primary = memList[0];

			using (var cmd = con.CreateCommand())
			{
				cmd.Transaction = tx;
				cmd.CommandText = @"
					UPDATE Devices SET
						Type=@Type,
						Manufacturer=@Man,
						Model=@Model,
						ImagePath=@Img,
						Resolution=@Res,
						DisplayType=@Disp,
						ScreenRefresh=@Ref,
						Processor=@Cpu,
						Ram=@Ram,
						Rom=@Rom,
						ChargeSpeed=@Chg,
						OsName=@Os,
						OsVersion=@Ver
					WHERE Id=@Id;
					";
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@Type", (int)device.Type);
				cmd.Parameters.AddWithValue("@Man", device.DeviceInfo.Manufacturer);
				cmd.Parameters.AddWithValue("@Model", device.DeviceInfo.Model);

				cmd.Parameters.AddWithValue("@Img",
					string.IsNullOrWhiteSpace(device.DeviceInfo.ImagePath)
						? DBNull.Value
						: device.DeviceInfo.ImagePath);

				cmd.Parameters.AddWithValue("@Res", device.DisplayInfo.Resolution);
				cmd.Parameters.AddWithValue("@Disp", (int)device.DisplayInfo.Type);
				cmd.Parameters.AddWithValue("@Ref", device.DisplayInfo.ScreenRefresh);

				cmd.Parameters.AddWithValue("@Cpu", device.HardwareInfo.Processor);
				cmd.Parameters.AddWithValue("@Ram", primary.Ram);
				cmd.Parameters.AddWithValue("@Rom", primary.Rom);
				cmd.Parameters.AddWithValue("@Chg", device.HardwareInfo.ChargeSpeed);

				cmd.Parameters.AddWithValue("@Os", device.SoftwareInfo.OsName);
				cmd.Parameters.AddWithValue("@Ver", device.SoftwareInfo.Version);

				cmd.ExecuteNonQuery();
			}

			// Дочерние коллекции (Cameras, MemoryConfigs) пересоздаются целиком,
			// чтобы избежать сложного сравнения и частичного обновления
			using (var del = con.CreateCommand())
			{
				del.Transaction = tx;
				del.CommandText = "DELETE FROM Cameras WHERE DeviceId=@Id;";
				del.Parameters.AddWithValue("@Id", id);
				del.ExecuteNonQuery();
			}

			using (var del = con.CreateCommand())
			{
				del.Transaction = tx;
				del.CommandText = "DELETE FROM MemoryConfigs WHERE DeviceId=@Id;";
				del.Parameters.AddWithValue("@Id", id);
				del.ExecuteNonQuery();
			}

			if (device.HardwareInfo.Cameras is { Count: > 0 })
			{
				using var cmd = con.CreateCommand();
				cmd.Transaction = tx;
				cmd.CommandText = "INSERT INTO Cameras(DeviceId, CameraType, Megapixels) VALUES (@Id,@T,@Mp);";
				var pId = cmd.Parameters.Add("@Id", SqliteType.Integer);
				var pT = cmd.Parameters.Add("@T", SqliteType.Integer);
				var pMp = cmd.Parameters.Add("@Mp", SqliteType.Integer);

				foreach (var c in device.HardwareInfo.Cameras)
				{
					pId.Value = id;
					pT.Value = (int)c.CameraType;
					pMp.Value = c.Megapixels;
					cmd.ExecuteNonQuery();
				}
			}
			{
				using var cmd = con.CreateCommand();
				cmd.Transaction = tx;
				cmd.CommandText = "INSERT INTO MemoryConfigs(DeviceId, Ram, Rom) VALUES (@Id,@Ram,@Rom);";
				var pId = cmd.Parameters.Add("@Id", SqliteType.Integer);
				var pRam = cmd.Parameters.Add("@Ram", SqliteType.Integer);
				var pRom = cmd.Parameters.Add("@Rom", SqliteType.Integer);

				foreach (var m in memList)
				{
					pId.Value = id;
					pRam.Value = m.Ram;
					pRom.Value = m.Rom;
					cmd.ExecuteNonQuery();
				}
			}

			tx.Commit();
		}
		public Task<List<Device>> SearchAsync(string userInput)
		{
			if (string.IsNullOrWhiteSpace(userInput))
				return GetAllAsync();

			userInput = userInput.Trim();

			using var con = new SqliteConnection(_cs);
			con.Open();

			// Фразовый поиск по модели
			var phraseIds = new List<int>();
			using (var phraseCmd = con.CreateCommand())
			{
				phraseCmd.CommandText = @"
					SELECT d.Id
					FROM Devices d
					WHERE
						d.Model LIKE @q COLLATE NOCASE
						OR (d.Manufacturer || ' ' || d.Model) LIKE @q COLLATE NOCASE
					ORDER BY d.Manufacturer, d.Model;
					";
				phraseCmd.Parameters.AddWithValue("@q", "%" + userInput + "%");

				using var r = phraseCmd.ExecuteReader();
				while (r.Read())
					phraseIds.Add(r.GetInt32(0));
			}

			if (phraseIds.Count > 0)
				return Task.FromResult(GetByIds(con, phraseIds));

			// Если фразу не нашли то ищем по словам
			var words = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (words.Length == 0)
				return GetAllAsync();

			var whereParts = new List<string>();
			using var cmd = con.CreateCommand();

			for (int i = 0; i < words.Length; i++)
			{
				var w = words[i];

				if (int.TryParse(w, out var n))
				{
					var p = $"@n{i}";
					cmd.Parameters.AddWithValue(p, n);

					whereParts.Add($@"
						(
							-- цифры в названии модели (Pixel 4)
							d.Model LIKE '%' || {p} || '%' COLLATE NOCASE

							OR d.Ram = {p}
							OR d.Rom = {p}
							OR d.ChargeSpeed = {p}

							OR EXISTS (
								SELECT 1 FROM Cameras c
								WHERE c.DeviceId = d.Id AND c.Megapixels = {p}
							)

							OR EXISTS (
								SELECT 1 FROM MemoryConfigs m
								WHERE m.DeviceId = d.Id AND (m.Ram = {p} OR m.Rom = {p})
							)
						)
						");
				}
				else
				{
					var p = $"@t{i}";
					cmd.Parameters.AddWithValue(p, "%" + w + "%");

					whereParts.Add($@"
						(
							d.Manufacturer LIKE {p} COLLATE NOCASE OR
							d.Model        LIKE {p} COLLATE NOCASE OR
							d.Processor    LIKE {p} COLLATE NOCASE OR
							d.OsName       LIKE {p} COLLATE NOCASE OR
							d.OsVersion    LIKE {p} COLLATE NOCASE

							-- если хочешь, можно оставить enum как текст (но часто это мусор)
							OR CAST(d.Type AS TEXT) LIKE {p}

							OR EXISTS (
								SELECT 1 FROM Cameras c
								WHERE c.DeviceId = d.Id
								  AND CAST(c.CameraType AS TEXT) LIKE {p}
							)
						)
						");
				}
			}

			cmd.CommandText = $@"
				SELECT d.Id
				FROM Devices d
				WHERE {string.Join(" AND ", whereParts)}
				ORDER BY d.Manufacturer, d.Model;
				";

			var ids = new List<int>();
			using (var r = cmd.ExecuteReader())
			{
				while (r.Read())
					ids.Add(r.GetInt32(0));
			}

			if (ids.Count == 0)
				return Task.FromResult(new List<Device>());

			return Task.FromResult(GetByIds(con, ids));
		}

		private List<Device> GetByIds(SqliteConnection con, List<int> ids)
		{
			var paramNames = ids.Select((_, i) => $"@id{i}").ToArray();
			var devices = new List<Device>();

			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = $@"
					SELECT
						Id, Type, Manufacturer, Model, ImagePath,
						Resolution, DisplayType, ScreenRefresh,
						Processor, Ram, Rom, ChargeSpeed,
						OsName, OsVersion
					FROM Devices
					WHERE Id IN ({string.Join(",", paramNames)});
					";

				for (int i = 0; i < ids.Count; i++)
					cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var id = r.GetInt32(0);

					var d = new Device(
						Id: id,
						Type: (DeviceType)r.GetInt32(1),
						DeviceInfo: new DeviceInfo(
							r.GetString(2),
							r.GetString(3),
							r.IsDBNull(4) ? "" : r.GetString(4)
						),
						DisplayInfo: new DisplayInfo(
							r.GetInt32(5),
							(DisplayType)r.GetInt32(6),
							r.GetInt32(7)
						),
						HardwareInfo: new HardwareInfo(
							Processor: r.GetString(8),
							Ram: r.GetInt32(9),
							Rom: r.GetInt32(10),
							ChargeSpeed: r.GetInt32(11),
							Cameras: new List<Camera>(),
							MemoryConfigs: new List<MemoryConfig>()
						),
						SoftwareInfo: new SoftwareInfo(
							r.GetString(12),
							r.GetString(13)
						)
					);

					devices.Add(d);
				}
			}

			if (devices.Count == 0) return devices;

			var idSet = new HashSet<int>(devices.Select(d => d.Id));

			var cams = new Dictionary<int, List<Camera>>();
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT DeviceId, CameraType, Megapixels FROM Cameras;";
				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var deviceId = r.GetInt32(0);
					if (!idSet.Contains(deviceId)) continue;

					if (!cams.TryGetValue(deviceId, out var list))
						cams[deviceId] = list = new List<Camera>();

					list.Add(new Camera((ECameraType)r.GetInt32(1), r.GetInt32(2)));
				}
			}

			var mems = new Dictionary<int, List<MemoryConfig>>();
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT DeviceId, Ram, Rom FROM MemoryConfigs;";
				using var r = cmd.ExecuteReader();
				while (r.Read())
				{
					var deviceId = r.GetInt32(0);
					if (!idSet.Contains(deviceId)) continue;

					if (!mems.TryGetValue(deviceId, out var list))
						mems[deviceId] = list = new List<MemoryConfig>();

					list.Add(new MemoryConfig(r.GetInt32(1), r.GetInt32(2)));
				}
			}

			for (int i = 0; i < devices.Count; i++)
			{
				var d = devices[i];

				var camList = cams.TryGetValue(d.Id, out var cl) ? cl : new List<Camera>();
				var memList = mems.TryGetValue(d.Id, out var ml) ? ml : new List<MemoryConfig>();

				if (memList.Count == 0)
					memList.Add(new MemoryConfig(d.HardwareInfo.Ram, d.HardwareInfo.Rom));

				var primary = memList[0];

				var hw = new HardwareInfo(
					Processor: d.HardwareInfo.Processor,
					Ram: primary.Ram,
					Rom: primary.Rom,
					ChargeSpeed: d.HardwareInfo.ChargeSpeed,
					Cameras: camList,
					MemoryConfigs: memList
				);

				devices[i] = new Device(d.Id, d.Type, d.DeviceInfo, d.DisplayInfo, hw, d.SoftwareInfo);
			}

			return devices;
		}

		// async адаптер
		public Task<List<Device>> GetAllAsync() => Task.FromResult(GetAll());
		public Task<int> AddAsync(Device device) => Task.FromResult(Add(device));
		public Task DeleteAsync(int id) { Delete(id); return Task.CompletedTask; }
		public Task UpdateAsync(int id, Device device) { Update(id, device); return Task.CompletedTask; }

		// Заполнение бд заготовленными девайсами из Seed
		public void SeedIfEmpty()
		{
			using var con = new SqliteConnection(_cs);
			con.Open();

			using var cmd = con.CreateCommand();
			cmd.CommandText = "SELECT COUNT(*) FROM Devices;";
			var count = Convert.ToInt32(cmd.ExecuteScalar());
			if (count > 0) return;

			foreach (var device in Seed.Devices)
				Add(device);
		}
	}
}