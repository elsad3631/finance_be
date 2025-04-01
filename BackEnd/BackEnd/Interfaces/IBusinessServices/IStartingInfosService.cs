using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IStartingInfosService
    {
        Task<IEnumerable<StartingInfos>> GetAsync();
        Task<StartingInfos> GetByIdAsync(string id);
        Task<StartingInfos> CreateAsync(StartingInfos item);
        Task UpdateAsync(string id, StartingInfos item);
        Task<bool> DeleteAsync(string id);
    }
}
