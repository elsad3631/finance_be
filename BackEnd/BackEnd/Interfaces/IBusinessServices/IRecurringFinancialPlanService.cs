using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRecurringFinancialPlanService
    {
        Task<IEnumerable<RecurringFinancialPlan>> GetAsync();
        Task<RecurringFinancialPlan> GetByIdAsync(string id);
        Task<RecurringFinancialPlan> CreateAsync(RecurringFinancialPlan item);
        Task UpdateAsync(string id, RecurringFinancialPlan item);
        Task<bool> DeleteAsync(string id);
    }
}
