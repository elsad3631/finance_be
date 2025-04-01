using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface ITransactionSummaryService
    {
        Task<IEnumerable<TransactionSummary>> GetAsync();
        Task<TransactionSummary> GetByIdAsync(string id);
        Task<TransactionSummary> CreateAsync(TransactionSummary item);
        Task UpdateAsync(string id, TransactionSummary item);
        Task<bool> DeleteAsync(string id);
    }
}
