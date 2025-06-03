using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IEmployeesService
    {

        Task<IEnumerable<Employee>> GetAsync();
        Task<Employee> GetByIdAsync(string id);
        Task<Employee> CreateAsync(Employee item);
        Task UpdateAsync(string id, Employee item);
        Task<bool> DeleteAsync(string id);
    }
}
