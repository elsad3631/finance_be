using BackEnd.CosmosEntities;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetAsync();
        Task<Wallet> GetByIdAsync(string id);
        Task<Wallet> CreateAsync(Wallet item);
        Task UpdateAsync(string id, Wallet item);
        Task<bool> DeleteAsync(string id);
    }
}
