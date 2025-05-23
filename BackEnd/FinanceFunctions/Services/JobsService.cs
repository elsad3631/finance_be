using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class JobsService : IJobsService
    {
        private readonly Container _container;
        public JobsService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<Job> CreateAsync(Job item)
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
                await _container.DeleteItemAsync<Job>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Job>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<Job>(queryDefinition);
            List<Job> results = new List<Job>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<Job> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Job> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<Job> response = await _container.ReadItemAsync<Job>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, Job item)
        {
            try
            {
                ItemResponse<Job> response = await _container.ReadItemAsync<Job>(id, new PartitionKey(id));
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
