using Device_Library_WPF.Models;

// Интерфейс для взаимодействия с sqlite дб
public interface IDeviceRepository
{
	Task<List<Device>> SearchAsync(string userInput);

	Task<List<Device>> GetAllAsync();
	Task<int> AddAsync(Device device);
	Task DeleteAsync(int id);
	Task UpdateAsync(int id, Device device);
}