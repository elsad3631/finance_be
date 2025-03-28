using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Services.BusinessServices;

namespace BackEnd.Services
{
    public static class ServicesStartup
    {

      
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<ICustomerServices, CustomerServices>();
        }
    }
}