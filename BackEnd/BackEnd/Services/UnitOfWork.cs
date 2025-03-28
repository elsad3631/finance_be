using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IRepositories;
using BackEnd.Services.Repositories;

namespace BackEnd.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public AppDbContext dbContext { get; set; }
        public UnitOfWork(AppDbContext context)
        {
            this._context = context;
            this.dbContext = context;

            CustomerRepository = new CustomerRepository(this._context);
        }

      
        public ICustomerRepository CustomerRepository
        {
            get;
            private set;
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();

        }

        public int Save()
        {
            return  _context.SaveChanges();

        }
    }
}