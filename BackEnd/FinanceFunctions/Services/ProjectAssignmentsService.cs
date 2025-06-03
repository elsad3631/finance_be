using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class ProjectAssignmentsService : IProjectAssignmentsService
    {
        private readonly Container _container;

        public ProjectAssignmentsService(string connection, string dbName, string containerName, string key)
        {
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);
            _container = cosmosClient.GetContainer(dbName, containerName);
        }

        public async Task<ProjectAssignment> CreateAsync(ProjectAssignment item)
        {
            try
            {
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;
                // La partition key per ProjectAssignment potrebbe essere ProjectId o EmployeeId,
                // a seconda delle query più frequenti. Usiamo ProjectId per ora.
                await _container.CreateItemAsync(item, new PartitionKey(item.ProjectId));
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating project assignment: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                // Per la delete, è necessario conoscere la partition key.
                // Una soluzione è recuperare l'elemento prima di cancellare,
                // o richiedere la partition key come parametro se conosciuta a priori.
                // Supponiamo di poter recuperare l'elemento per trovare la partition key.
                var itemToDelete = await GetByIdAsync(id);
                if (itemToDelete == null) return false;

                await _container.DeleteItemAsync<ProjectAssignment>(id, new PartitionKey(itemToDelete.ProjectId));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting project assignment with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProjectAssignment>> GetAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var resultSet = _container.GetItemQueryIterator<ProjectAssignment>(queryDefinition);
            List<ProjectAssignment> results = new List<ProjectAssignment>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<ProjectAssignment> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<ProjectAssignment> GetByIdAsync(string id)
        {
            try
            {
                // Nota: Per GetById, se la partition key non è l'ID stesso,
                // è necessario effettuare una query su tutti gli elementi (meno efficiente)
                // o conoscere la partition key.
                // Per semplicità, qui facciamo una query. In produzione, valutare l'architettura.
                var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", id);
                var resultSet = _container.GetItemQueryIterator<ProjectAssignment>(queryDefinition);
                List<ProjectAssignment> results = new List<ProjectAssignment>();
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<ProjectAssignment> response = await resultSet.ReadNextAsync();
                    results.AddRange(response);
                }
                return results.FirstOrDefault(); // Dovrebbe essercene uno solo
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting project assignment with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(string id, ProjectAssignment item)
        {
            try
            {
                // Simile a GetByIdAsync, potremmo aver bisogno di recuperare l'elemento
                // per conoscere la sua PartitionKey se non è l'ID.
                // In un'applicazione reale, il client dovrebbe fornire la partition key per gli aggiornamenti.
                ItemResponse<ProjectAssignment> existingItemResponse = await _container.ReadItemAsync<ProjectAssignment>(id, new PartitionKey(item.ProjectId));
                ProjectAssignment existingItem = existingItemResponse.Resource;

                item.Id = id;
                item.CreationDate = existingItem.CreationDate;
                item.UpdateDate = DateTime.UtcNow;

                await _container.UpsertItemAsync(item, new PartitionKey(item.ProjectId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating project assignment with ID {id}: {ex.Message}", ex);
            }
        }

        // Nuove query specifiche
        public async Task<IEnumerable<ProjectAssignment>> GetByEmployeeIdAsync(string employeeId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.employee_id = @employeeId")
                .WithParameter("@employeeId", employeeId);
            var resultSet = _container.GetItemQueryIterator<ProjectAssignment>(queryDefinition);
            List<ProjectAssignment> results = new List<ProjectAssignment>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<ProjectAssignment> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IEnumerable<ProjectAssignment>> GetByProjectIdAsync(string projectId)
        {
            // Se ProjectId è la partition key, questa query sarà molto efficiente.
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.project_id = @projectId")
                .WithParameter("@projectId", projectId);
            var resultSet = _container.GetItemQueryIterator<ProjectAssignment>(queryDefinition, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(projectId) });
            List<ProjectAssignment> results = new List<ProjectAssignment>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<ProjectAssignment> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }
    }
}
