using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IProjectAssignmentsService
    {
        Task<IEnumerable<ProjectAssignment>> GetAsync();
        Task<ProjectAssignment> GetByIdAsync(string id);
        Task<ProjectAssignment> CreateAsync(ProjectAssignment item);
        Task UpdateAsync(string id, ProjectAssignment item);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<ProjectAssignment>> GetByEmployeeIdAsync(string employeeId); // Nuova query utile
        Task<IEnumerable<ProjectAssignment>> GetByProjectIdAsync(string projectId); // Nuova query utile
    }
}
