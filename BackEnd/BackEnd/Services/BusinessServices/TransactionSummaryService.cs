using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class TransactionSummaryService : ITransactionSummaryService
    {
        private readonly Container _container;
        public TransactionSummaryService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<TransactionSummary> CreateAsync(TransactionSummary item)
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
                await _container.DeleteItemAsync<TransactionSummary>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<TransactionSummary>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<TransactionSummary>(queryDefinition);
            List<TransactionSummary> results = new List<TransactionSummary>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<TransactionSummary> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<TransactionSummary> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<TransactionSummary> response = await _container.ReadItemAsync<TransactionSummary>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, TransactionSummary item)
        {
            try
            {
                ItemResponse<TransactionSummary> response = await _container.ReadItemAsync<TransactionSummary>(id, new PartitionKey(id));
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
