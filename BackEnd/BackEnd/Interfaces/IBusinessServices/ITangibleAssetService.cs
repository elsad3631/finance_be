using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface ITangibleAssetService
    {
        Task<IEnumerable<TangibleAsset>> GetAsync();
        Task<TangibleAsset> GetByIdAsync(string id);
        Task<TangibleAsset> CreateAsync(TangibleAsset item);
        Task UpdateAsync(string id, TangibleAsset item);
        Task<bool> DeleteAsync(string id);
    }
}
