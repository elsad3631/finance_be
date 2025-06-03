using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly Container _container;
        public ProjectsService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<Project> CreateAsync(Project item)
        {
            try
            {
                // Imposta CreationDate prima della creazione
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;
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
                await _container.DeleteItemAsync<Project>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Project>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<Project>(queryDefinition);
            List<Project> results = new List<Project>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<Project> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Project> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<Project> response = await _container.ReadItemAsync<Project>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, Project item)
        {
            try
            {
                ItemResponse<Project> response = await _container.ReadItemAsync<Project>(id, new PartitionKey(id));
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
