using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class TangibleAssetService : ITangibleAssetService
    {
        private readonly Container _container;
        public TangibleAssetService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<TangibleAsset> CreateAsync(TangibleAsset item)
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
                await _container.DeleteItemAsync<TangibleAsset>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<TangibleAsset>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<TangibleAsset>(queryDefinition);
            List<TangibleAsset> results = new List<TangibleAsset>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<TangibleAsset> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<TangibleAsset> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<TangibleAsset> response = await _container.ReadItemAsync<TangibleAsset>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, TangibleAsset item)
        {
            try
            {
                ItemResponse<TangibleAsset> response = await _container.ReadItemAsync<TangibleAsset>(id, new PartitionKey(id));
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
