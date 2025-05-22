using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IResourcesService
    {
        Task<IEnumerable<Resource>> GetAsync();
        Task<Resource> GetByIdAsync(string id);
        Task<Resource> CreateAsync(Resource item);
        Task UpdateAsync(string id, Resource item);
        Task<bool> DeleteAsync(string id);
    }
}
