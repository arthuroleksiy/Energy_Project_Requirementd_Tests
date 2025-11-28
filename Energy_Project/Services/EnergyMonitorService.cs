
using Energy_Project.Services.Interfaces;
using Energy_Project.Models;

namespace Energy_Project.Services
{
    public class EnergyMonitorService
    {
        private readonly IDeviceRepository _devices;
        private readonly IEnergyPlanRepository _plans;
        private readonly INotificationService _notify;
        private readonly IUsageLogRepository _usageLog;

        public bool AlertsEnabled { get; set; } = true;

        private bool _overloadAlertSentToday = false;

        private double _dailyUsageAccumulated = 0.0;

        public EnergyMonitorService(
            IDeviceRepository devices,
            IEnergyPlanRepository plans,
            INotificationService notify,
            IUsageLogRepository usageLog)
        {
            _devices = devices;
            _plans = plans;
            _notify = notify;
            _usageLog = usageLog;
        }

        public double CalculateCurrentUsageKwh()
        {
            return _devices.GetAll()
                .Where(d => d.IsOn)
                .Sum(d => d.PowerUsageWatts) / 1000.0;
        }

        public void RecordUsageForInterval(double hours)
        {
            var activeDevices = _devices.GetAll().Where(d => d.IsOn);

            foreach (var d in activeDevices)
            {
                double wh = d.PowerUsageWatts * hours;
                _usageLog.LogDeviceUsage(d.Id, wh);
            }

            _dailyUsageAccumulated += activeDevices.Sum(d => d.PowerUsageWatts * hours) / 1000.0;
        }

        public void CheckForOverload()
        {
            if (!AlertsEnabled)
                return;

            var usage = CalculateCurrentUsageKwh();
            var plan = _plans.GetCurrentPlan()
                       ?? throw new InvalidOperationException("No current plan configured.");

            if (usage > plan.DailyLimitKwh)
            {
                if (!_overloadAlertSentToday)
                {
                    _notify.SendAlert($"Overload detected: current usage {usage:F2} kWh!");
                    _overloadAlertSentToday = true;
                }
            }
            else
            {
                _overloadAlertSentToday = false;
            }
        }

        public void ResetDailyUsage()
        {
            _dailyUsageAccumulated = 0.0;
            _overloadAlertSentToday = false;
        }

        public void UpdateEnergyLimit(double newLimit)
        {
            if (newLimit < 0.1 || newLimit > 1000)
                throw new ArgumentOutOfRangeException("New limit must be between 0.1 and 1000 kWh.");

            var plan = _plans.GetCurrentPlan()
                       ?? throw new InvalidOperationException("Plan not found.");

            double oldLimit = plan.DailyLimitKwh;

            plan.DailyLimitKwh = newLimit;
            _plans.UpdatePlan(plan);

            _plans.RecordPlanChange(oldLimit, newLimit);

            _notify.SendAlert($"Energy limit updated to {newLimit:F2} kWh.");
        }

        public double GetDailyUsage() => _dailyUsageAccumulated;
    }
}
