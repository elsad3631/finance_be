using Azure;
using Azure.AI.OpenAI;
using Azure.Storage.Blobs;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using FinanceFunctions.Models.InputModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace FinanceFunctions.Functions
{
    public class ResourcesFunctions
    {
        private readonly IEmployeesService _employeesService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAzureAIService _azureAIService;

        public ResourcesFunctions(IEmployeesService employeesService, IBlobStorageService blobStorageService, IAzureAIService azureAIService)
        {
            _employeesService = employeesService;
            _blobStorageService = blobStorageService;
            _azureAIService = azureAIService;
        }

        [FunctionName("Insert")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Resource/Insert")] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Controllo se c'è un file in input
                if (!req.HasFormContentType || req.Form.Files.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("File non fornito.")
                    };
                }

                var file = req.Form.Files[0];

                Employee newResource = await _employeesService.CreateAsync(new Employee());
                string newResourceId = newResource.Id;

                string fileName = $"{newResourceId}/{file.FileName.Replace(" ", "_")}";

                using (var fileStream = file.OpenReadStream())
                {
                    string fileUrl = await _blobStorageService.UploadFileAsync(fileStream, fileName);
                }

                var memoryStream = await _blobStorageService.DownloadFileAsync(fileName);
                memoryStream.Position = 0;

                string extractedText = string.Empty;
                string extension = Path.GetExtension(file.FileName).ToLower();
                if (extension.Contains("docx"))
                {
                    using var doc = DocX.Load(memoryStream);
                    extractedText = doc.Text;
                }
                else
                {
                    throw new NotSupportedException("Tipo file non supportato. Solo DOCX.");
                }

                Employee resourceData = await _azureAIService.GetCVData(extractedText);
                //newResource = JsonSerializer.Deserialize<Employee>(resourceData);
                newResource.Id = newResourceId;
                //newResource.CVName = fileName;

                await _employeesService.UpdateAsync(newResourceId, newResource);

                log.LogInformation("Resource Inserted");
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                log.LogError("Errore: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };
            }
        }

        [FunctionName("UploadCVAndExtractData")]
        public async Task<HttpResponseMessage> UploadCVAndExtractData(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "employees/upload-cv")] HttpRequest req,
        ILogger log)
        {
            try
            {
                // Controllo se c'è un file in input
                if (!req.HasFormContentType || req.Form.Files.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("File non fornito.")
                    };
                }

                var file = req.Form.Files[0];
                
                //var memoryStream = await _blobStorageService.DownloadFileAsync(fileName);
                var memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                string extractedText = string.Empty;
                string extension = Path.GetExtension(file.FileName).ToLower();
                if (extension.Contains("docx"))
                {
                    using var doc = DocX.Load(memoryStream);
                    extractedText = doc.Text;
                }
                else
                {
                    throw new NotSupportedException("Tipo file non supportato. Solo DOCX.");
                }

                // Chiama il servizio AI per estrarre i dati del CV
                // Assumiamo che GetCVData ritorni un JSON che mappa all'entità Employee
                Employee employeeData = await _azureAIService.GetCVData(extractedText);

                Employee newResource = await _employeesService.CreateAsync(new Employee());
                string newResourceId = newResource.Id;

                string fileName = $"{newResourceId}/{file.FileName.Replace(" ", "_")}";

                string storageUrl = string.Empty;
                using (var fileStream = file.OpenReadStream())
                {
                    storageUrl = await _blobStorageService.UploadFileAsync(fileStream, fileName);
                }

                // Imposta i metadati del CV
                employeeData.CVData = new CVData
                {
                    FileName = fileName,
                    StorageUrl = storageUrl,
                    UploadDate = DateTime.UtcNow,
                    ParsedVersion = 1 // Inizializza la versione
                };

                employeeData.Id = newResourceId;
                await _employeesService.UpdateAsync(newResourceId, employeeData);

                log.LogInformation($"Employee profile created from CV: {employeeData.FirstName} {employeeData.LastName}");
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(employeeData))
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error in UploadCVAndExtractData: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetEmployees")]
        public async Task<HttpResponseMessage> GetEmployees(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var employees = await _employeesService.GetAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(employees), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting employees: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetEmployeeById")]
        public async Task<HttpResponseMessage> GetEmployeeById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var employee = await _employeesService.GetByIdAsync(id);
                if (employee == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(employee), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting employee by ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("UpdateEmployee")]
        public async Task<HttpResponseMessage> UpdateEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "employees/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var employeeInput = System.Text.Json.JsonSerializer.Deserialize<Employee>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await _employeesService.UpdateAsync(id, employeeInput);

                log.LogInformation($"Employee with ID {id} updated successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating employee with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("DeleteEmployee")]
        public async Task<HttpResponseMessage> DeleteEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "employees/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                bool deleted = await _employeesService.DeleteAsync(id);
                if (!deleted)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                log.LogInformation($"Employee with ID {id} deleted successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting employee with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }
    }
}
