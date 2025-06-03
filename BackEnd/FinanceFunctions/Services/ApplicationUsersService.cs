using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using FinanceFunctions.CosmosEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class ApplicationUsersService : IApplicationUsersService
    {
        private readonly Container _container;

        public ApplicationUsersService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);
            _container = cosmosClient.GetContainer(dbName, containerName);
        }

        public async Task<ApplicationUser> CreateAsync(ApplicationUser item)
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
                throw new Exception($"Error creating ApplicationUser: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<ApplicationUser>(id, new PartitionKey(id)); // Usa id come PartitionKey
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting ApplicationUser with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<ApplicationUser>(queryDefinition);
            List<ApplicationUser> results = new List<ApplicationUser>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<ApplicationUser> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<ApplicationUser> response = await _container.ReadItemAsync<ApplicationUser>(id, new PartitionKey(id)); // Usa id come PartitionKey
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting ApplicationUser with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(string id, ApplicationUser item)
        {
            try
            {
                ItemResponse<ApplicationUser> existingItemResponse = await _container.ReadItemAsync<ApplicationUser>(id, new PartitionKey(id));
                ApplicationUser existingItem = existingItemResponse.Resource;

                item.Id = id;
                item.CreationDate = existingItem.CreationDate;
                item.UpdateDate = DateTime.UtcNow;

                await _container.UpsertItemAsync(item, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating ApplicationUser with ID {id}: {ex.Message}", ex);
            }
        }

        // Nuova query specifica per trovare un utente tramite email
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
                .WithParameter("@email", email);
            var resultSet = _container.GetItemQueryIterator<ApplicationUser>(queryDefinition);
            List<ApplicationUser> results = new List<ApplicationUser>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<ApplicationUser> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results.FirstOrDefault(); // L'email dovrebbe essere unica
        }
    }
}
