using Energy_Project.Services.Interfaces;
using Moq;
using Energy_Project.Models;
using Energy_Project.Services;
using Energy_Project.Services.Interfaces;
using Moq;

namespace SmartHomeTests
{
    public class DeviceServiceRequirementsTests
    {
        private readonly Mock<IDeviceRepository> _repo = new();

        private DeviceService CreateService() => new DeviceService(_repo.Object);

    }


}
