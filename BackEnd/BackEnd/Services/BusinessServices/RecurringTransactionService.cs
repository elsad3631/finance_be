using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly Container _container;
        public RecurringTransactionService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<RecurringTransaction> CreateAsync(RecurringTransaction item)
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
                await _container.DeleteItemAsync<RecurringTransaction>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<RecurringTransaction>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<RecurringTransaction>(queryDefinition);
            List<RecurringTransaction> results = new List<RecurringTransaction>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<RecurringTransaction> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<RecurringTransaction> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<RecurringTransaction> response = await _container.ReadItemAsync<RecurringTransaction>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, RecurringTransaction item)
        {
            try
            {
                ItemResponse<RecurringTransaction> response = await _container.ReadItemAsync<RecurringTransaction>(id, new PartitionKey(id));
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
