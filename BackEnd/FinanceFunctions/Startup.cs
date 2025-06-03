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

            builder.Services.AddScoped<IApplicationUsersService, ApplicationUsersService>();

            builder.Services.AddSingleton<IBlobStorageService>(provider =>
                    new BlobStorageService(configuration["AzureBlobStorageConnectionString"], configuration["AzureBlobStorageContainer"]));

            builder.Services.AddSingleton<IAzureAIService>(provider =>
                    new AzureAIService(configuration["OPENAI_ENDPOINT"], configuration["OPENAI_KEY"], configuration["OPENAI_DEPLOYMENT"]));

            builder.Services.AddScoped<INewsService, NewsService>(x =>
            new NewsService(
               configuration["CosmosDb_ConnectionString"]!,
               configuration["CosmosDb_DatabaseName"]!,
               configuration["CosmosDb_Containers_NewsContainer"]!,
               configuration["CosmosDb_Key"]!
            ));

            builder.Services.AddScoped<IEmployeesService, EmployeesService>(x =>
           new EmployeesService(
              configuration["CosmosDb_ConnectionString"]!,
              configuration["CosmosDb_DatabaseName"]!,
              configuration["CosmosDb_Containers_EmployeesContainer"]!,
              configuration["CosmosDb_Key"]!
           ));

            builder.Services.AddScoped<IProjectsService, ProjectsService>(x =>
            new ProjectsService(
               configuration["CosmosDb_ConnectionString"]!,
               configuration["CosmosDb_DatabaseName"]!,
               configuration["CosmosDb_Containers_ProjectsContainer"]!,
               configuration["CosmosDb_Key"]!
            ));

            builder.Services.AddScoped<IProjectAssignmentsService, ProjectAssignmentsService>(x =>
           new ProjectAssignmentsService(
              configuration["CosmosDb_ConnectionString"]!,
              configuration["CosmosDb_DatabaseName"]!,
              configuration["CosmosDb_Containers_ProjectAssignmentsContainer"]!,
              configuration["CosmosDb_Key"]!
           ));

            builder.Services.AddScoped<ISkillTrainingsService, SkillTrainingsService>();
        }
    }
}
