using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceFunctions.IServices;
using FinanceFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(FinanceFunctions.Startup))]

namespace FinanceFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddScoped<INewsService, NewsService>(x =>
            new NewsService(
               configuration["CosmosDb_ConnectionString"]!,
               configuration["CosmosDb_DatabaseName"]!,
               configuration["CosmosDb_Containers_NewsContainer"]!,
               configuration["CosmosDb_Key"]!
            ));

            builder.Services.AddScoped<IResourcesService, ResourcesService>(x =>
           new ResourcesService(
              configuration["CosmosDb_ConnectionString"]!,
              configuration["CosmosDb_DatabaseName"]!,
              configuration["CosmosDb_Containers_ResourcesContainer"]!,
              configuration["CosmosDb_Key"]!
           ));
        }
    }
}
