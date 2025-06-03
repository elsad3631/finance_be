using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class SkillTrainingsService : ISkillTrainingsService
    {
        private readonly Container _container;

        public SkillTrainingsService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);
            _container = cosmosClient.GetContainer(dbName, containerName);
        }

        public async Task<SkillTraining> CreateAsync(SkillTraining item)
        {
            try
            {
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;
                await _container.CreateItemAsync(item, new PartitionKey(item.Id)); // Usa item.Id come PartitionKey
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating skill training: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<SkillTraining>(id, new PartitionKey(id)); // Usa id come PartitionKey
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting skill training with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SkillTraining>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<SkillTraining>(queryDefinition);
            List<SkillTraining> results = new List<SkillTraining>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<SkillTraining> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<SkillTraining> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<SkillTraining> response = await _container.ReadItemAsync<SkillTraining>(id, new PartitionKey(id)); // Usa id come PartitionKey
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting skill training with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(string id, SkillTraining item)
        {
            try
            {
                ItemResponse<SkillTraining> existingItemResponse = await _container.ReadItemAsync<SkillTraining>(id, new PartitionKey(id));
                SkillTraining existingItem = existingItemResponse.Resource;

                item.Id = id;
                item.CreationDate = existingItem.CreationDate;
                item.UpdateDate = DateTime.UtcNow;

                await _container.UpsertItemAsync(item, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating skill training with ID {id}: {ex.Message}", ex);
            }
        }

        // Nuova query specifica
        public async Task<IEnumerable<SkillTraining>> GetBySkillDevelopedAsync(string skillName)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE ARRAY_CONTAINS(c.skills_developed, @skillName)")
                .WithParameter("@skillName", skillName);
            var resultSet = _container.GetItemQueryIterator<SkillTraining>(queryDefinition);
            List<SkillTraining> results = new List<SkillTraining>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<SkillTraining> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }
    }
}
