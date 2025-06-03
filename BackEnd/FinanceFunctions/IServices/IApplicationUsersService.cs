using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IApplicationUsersService
    {
        Task<IEnumerable<ApplicationUser>> GetAsync();
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> CreateAsync(ApplicationUser item);
        Task UpdateAsync(string id, ApplicationUser item);
        Task<bool> DeleteAsync(string id);
        Task<ApplicationUser> GetByEmailAsync(string email); // Nuova query utile per il login
    }
}
