using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmployeesService : IEmployeesService
{
    private readonly Container _container;

    public EmployeesService(string connection, string dbName, string containerName, string key)
    {
        CosmosClient cosmosClient = new CosmosClient(accountEndpoint: connection, authKeyOrResourceToken: key);
        _container = cosmosClient.GetContainer(dbName, containerName);
    }

    public async Task<Employee> CreateAsync(Employee item)
    {
        try
        {
            // Imposta CreationDate prima della creazione
            item.CreationDate = DateTime.UtcNow;
            item.UpdateDate = DateTime.UtcNow;
            await _container.CreateItemAsync(item, new PartitionKey(item.Id)); // Usa item.Id come PartitionKey
            return item;
        }
        catch (Exception ex)
        {
            // Puoi loggare l'errore qui per una migliore diagnostica
            throw new Exception($"Error creating employee: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Employee>(id, new PartitionKey(id)); // Usa id come PartitionKey
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting employee with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Employee>> GetAsync()
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        var resultSet = _container.GetItemQueryIterator<Employee>(queryDefinition);
        List<Employee> results = new List<Employee>();

        while (resultSet.HasMoreResults)
        {
            FeedResponse<Employee> response = await resultSet.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Employee> GetByIdAsync(string id)
    {
        try
        {
            ItemResponse<Employee> response = await _container.ReadItemAsync<Employee>(id, new PartitionKey(id)); // Usa id come PartitionKey
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null; // Ritorna null se l'elemento non viene trovato
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting employee with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(string id, Employee item)
    {
        try
        {
            // Recupera l'elemento esistente per preservare CreationDate e altre proprietà non aggiornate dal client
            ItemResponse<Employee> existingItemResponse = await _container.ReadItemAsync<Employee>(id, new PartitionKey(id));
            Employee existingItem = existingItemResponse.Resource;

            // Aggiorna le proprietà dell'elemento esistente con quelle dell'item fornito
            // e imposta la data di aggiornamento.
            item.Id = id; // Assicurati che l'ID sia corretto per l'aggiornamento
            item.CreationDate = existingItem.CreationDate; // Mantiene la data di creazione originale
            item.UpdateDate = DateTime.UtcNow; // Aggiorna la data di modifica

            await _container.UpsertItemAsync(item, new PartitionKey(id)); // Upsert per aggiornare
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating employee with ID {id}: {ex.Message}", ex);
        }
    }
}