using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class WalletService : IWalletService
    {
        private readonly Container _container;
        public WalletService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<Wallet> CreateAsync(Wallet item)
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
                await _container.DeleteItemAsync<Wallet>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Wallet>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<Wallet>(queryDefinition);
            List<Wallet> results = new List<Wallet>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<Wallet> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Wallet> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<Wallet> response = await _container.ReadItemAsync<Wallet>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, Wallet item)
        {
            try
            {
                ItemResponse<Wallet> response = await _container.ReadItemAsync<Wallet>(id, new PartitionKey(id));
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
