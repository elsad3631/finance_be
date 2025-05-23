using System.Threading.Tasks;

namespace FinanceFunctions.IServices
{
    public interface IAzureAIService
    {
        Task<string> GetResourceData(string cvText);
        Task<string> GetJobData(string title, string description);
    }
}
