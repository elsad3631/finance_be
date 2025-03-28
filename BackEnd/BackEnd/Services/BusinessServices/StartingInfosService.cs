using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.UserInformation;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Services.BusinessServices
{
    public class StartingInfosService : IStartingInfosService
    {
        private readonly Container _container;
        private readonly string containerName = "starting_infos";
        public StartingInfosService(string connection, string dbName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken:key);

            _container = cosmosClient.GetContainer(dbName, containerName);
        }
        public async Task<StartingInfos> CreateAsync(StartingInfos item)
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
                await _container.DeleteItemAsync<StartingInfos>(id, new PartitionKey(id));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<StartingInfos>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<StartingInfos>(queryDefinition);
            List<StartingInfos> results = new List<StartingInfos>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<StartingInfos> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<StartingInfos> GetByIdAsync(string id)
        {
            try
            {                
                ItemResponse<StartingInfos> response = await _container.ReadItemAsync<StartingInfos>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(string id, StartingInfos item)
        {
            try
            {
                ItemResponse<StartingInfos> response = await _container.ReadItemAsync<StartingInfos>(id, new PartitionKey(id));
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
