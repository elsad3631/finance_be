using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Services.BusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services
{
    public static class ServicesStartup
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IStartingInfosService, StartingInfosService>(x => 
            new StartingInfosService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Key"]!));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}