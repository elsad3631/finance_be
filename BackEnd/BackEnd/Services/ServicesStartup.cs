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
                builder.Configuration["CosmosDb:Containers:StartingInfosContainer"]!,
                builder.Configuration["CosmosDb:Key"]!
                ));

            builder.Services.AddScoped<IRecurringFinancialPlanService, RecurringFinancialPlanService>(x =>
            new RecurringFinancialPlanService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Containers:RecurringFinancialPlanContainer"]!,
                builder.Configuration["CosmosDb:Key"]!));

            builder.Services.AddScoped<IRecurringTransactionService, RecurringTransactionService>(x =>
            new RecurringTransactionService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Containers:RecurringTransactionContainer"]!,
                builder.Configuration["CosmosDb:Key"]!));

            builder.Services.AddScoped<ITangibleAssetService, TangibleAssetService>(x =>
            new TangibleAssetService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Containers:TangibleAssetContainer"]!,
                builder.Configuration["CosmosDb:Key"]!));

            builder.Services.AddScoped<ITransactionSummaryService, TransactionSummaryService>(x =>
            new TransactionSummaryService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Containers:TransactionSummaryContainer"]!,
                builder.Configuration["CosmosDb:Key"]!));

            builder.Services.AddScoped<IWalletService, WalletService>(x =>
            new WalletService(
                builder.Configuration["CosmosDb:ConnectionString"]!,
                builder.Configuration["CosmosDb:DatabaseName"]!,
                builder.Configuration["CosmosDb:Containers:WalletContainer"]!,
                builder.Configuration["CosmosDb:Key"]!));

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}