namespace Energy_Project.Services.Interfaces
{
    public interface IUsageLogRepository
    {
        void LogDeviceUsage(int Id, double wh);
    }
}
