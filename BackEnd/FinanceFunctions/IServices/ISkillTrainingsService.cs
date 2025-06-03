using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface ISkillTrainingsService
    {
        Task<IEnumerable<SkillTraining>> GetAsync();
        Task<SkillTraining> GetByIdAsync(string id);
        Task<SkillTraining> CreateAsync(SkillTraining item);
        Task UpdateAsync(string id, SkillTraining item);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<SkillTraining>> GetBySkillDevelopedAsync(string skillName); // Nuova query utile
    }
}
