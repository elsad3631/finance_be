using BackEnd.Data;
using BackEnd.Interfaces.IRepositories;

namespace BackEnd.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        AppDbContext dbContext { get; }
        
        Task<int> SaveAsync();
        int Save();

    }
}