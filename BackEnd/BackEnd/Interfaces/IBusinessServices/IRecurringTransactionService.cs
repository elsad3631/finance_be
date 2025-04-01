using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRecurringTransactionService
    {
        Task<IEnumerable<RecurringTransaction>> GetAsync();
        Task<RecurringTransaction> GetByIdAsync(string id);
        Task<RecurringTransaction> CreateAsync(RecurringTransaction item);
        Task UpdateAsync(string id, RecurringTransaction item);
        Task<bool> DeleteAsync(string id);
    }
}
