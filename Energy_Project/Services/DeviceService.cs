
using Energy_Project.Services.Interfaces;
using Energy_Project.Models;

namespace Energy_Project.Services
{
    public class DeviceService
    {
        private readonly IDeviceRepository _repo;

        private const int MaxWattThreshold = 3000;

        public DeviceService(IDeviceRepository repo)
        {
            _repo = repo;
        }

        public bool ToggleDevice(int id, bool turnOn)
        {
            var device = _repo.GetById(id)
                         ?? throw new ArgumentException("Device not found");

            if (turnOn && device.PowerUsageWatts > MaxWattThreshold)
                throw new InvalidOperationException("Device exceeds maximum allowed wattage.");

            device.IsOn = turnOn;
            device.LastToggledAt = DateTime.UtcNow;

            _repo.Update(device);
            return device.IsOn;
        }

        public int GetActiveDeviceCount()
        {
            return _repo.GetAll().Count(d => d.IsOn);
        }

        public IEnumerable<Device> GetActiveDevices()
        {
            return _repo.GetAll().Where(d => d.IsOn);
        }
    }
}
