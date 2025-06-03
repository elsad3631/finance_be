using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IProjectsService
    {
        Task<IEnumerable<Project>> GetAsync();
        Task<Project> GetByIdAsync(string id);
        Task<Project> CreateAsync(Project item);
        Task UpdateAsync(string id, Project item);
        Task<bool> DeleteAsync(string id);
    }
}
