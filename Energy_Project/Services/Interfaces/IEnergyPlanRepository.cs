using Energy_Project.Models;

namespace Energy_Project.Services.Interfaces
{
    public interface IEnergyPlanRepository
    {
        EnergyPlan GetCurrentPlan();
        void UpdatePlan(EnergyPlan plan);
        void RecordPlanChange(double oldLimit, double newLimit);
    }

}
