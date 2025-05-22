using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class NewsService : INewsService
    {
        private readonly Container _container;
        public NewsService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<BraveSearch> CreateAsync(BraveSearch item)
        {
            try
            {
                item.CreationDate = DateTime.Now.ToString();
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
                await _container.DeleteItemAsync<BraveSearch>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<BraveSearch>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<BraveSearch>(queryDefinition);
            List<BraveSearch> results = new List<BraveSearch>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<BraveSearch> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<BraveSearch> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<BraveSearch> response = await _container.ReadItemAsync<BraveSearch>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, BraveSearch item)
        {
            try
            {
                ItemResponse<BraveSearch> response = await _container.ReadItemAsync<BraveSearch>(id, new PartitionKey(id));
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
