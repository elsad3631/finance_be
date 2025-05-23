using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IJobsService
    {
        Task<IEnumerable<Job>> GetAsync();
        Task<Job> GetByIdAsync(string id);
        Task<Job> CreateAsync(Job item);
        Task UpdateAsync(string id, Job item);
        Task<bool> DeleteAsync(string id);
    }
}
