using Xunit;
using Moq;
using Energy_Project.Services;
using Energy_Project.Services.Interfaces;
using Energy_Project.Models;

namespace SmartHomeTests
{
    public class EnergyMonitorRequirementsTests
    {
        private readonly Mock<IDeviceRepository> _devices = new();
        private readonly Mock<IEnergyPlanRepository> _plans = new();
        private readonly Mock<INotificationService> _notify = new();
        private readonly Mock<IUsageLogRepository> _usageLog = new();

        private EnergyMonitorService CreateService()
            => new EnergyMonitorService(_devices.Object, _plans.Object, _notify.Object, _usageLog.Object);
    }
}
