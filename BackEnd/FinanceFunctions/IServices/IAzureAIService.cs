using FinanceFunctions.CosmosEntities;
using FinanceFunctions.Models.EmployeeModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IAzureAIService
    {
        //Task<string> GetCVData(string cvText);
        //Task<string> GetProjectSkills(string title, string description);

        // Modificato per restituire un oggetto Employee tipizzato
        Task<Employee> GetCVData(string cvText);

        // Modificato per restituire un oggetto Project tipizzato con le skill estratte
        Task<Project> GetProjectSkills(string title, string description);

        // Nuovo metodo per il matching di dipendenti a un progetto
        Task<List<EmployeeMatchResult>> GetBestMatchesForProject(Project project, IEnumerable<Employee> availableEmployees);

        // Nuovo metodo per suggerire corsi di formazione a un dipendente
        Task<List<SkillTraining>> GetSuggestedTrainings(Employee employee, IEnumerable<SkillTraining> allTrainings);
    }
}
