using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface INewsService
    {
        Task<IEnumerable<BraveSearch>> GetAsync();
        Task<BraveSearch> GetByIdAsync(string id);
        Task<BraveSearch> CreateAsync(BraveSearch item);
        Task UpdateAsync(string id, BraveSearch item);
        Task<bool> DeleteAsync(string id);
    }
}
