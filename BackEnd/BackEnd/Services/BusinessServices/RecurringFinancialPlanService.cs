using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class RecurringFinancialPlanService : IRecurringFinancialPlanService
    {
        private readonly Container _container;
        public RecurringFinancialPlanService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<RecurringFinancialPlan> CreateAsync(RecurringFinancialPlan item)
        {
            try
            {
                await _container.CreateItemAsync(item, new PartitionKey(item.Id));
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<RecurringFinancialPlan>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<RecurringFinancialPlan>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<RecurringFinancialPlan>(queryDefinition);
            List<RecurringFinancialPlan> results = new List<RecurringFinancialPlan>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<RecurringFinancialPlan> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<RecurringFinancialPlan> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<RecurringFinancialPlan> response = await _container.ReadItemAsync<RecurringFinancialPlan>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, RecurringFinancialPlan item)
        {
            try
            {
                ItemResponse<RecurringFinancialPlan> response = await _container.ReadItemAsync<RecurringFinancialPlan>(id, new PartitionKey(id));
                item.Id = id;
                item.CreationDate = response.Resource.CreationDate;
                await _container.UpsertItemAsync(item, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
