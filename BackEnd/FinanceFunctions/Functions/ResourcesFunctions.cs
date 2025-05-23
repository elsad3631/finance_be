using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xceed.Words.NET;
using System.Net;
using System.Text.Json;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Azure.Storage.Blobs;
using System.Configuration;

namespace FinanceFunctions.Functions
{
    public class ResourcesFunctions
    {
        private readonly IResourcesService _resourcesService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAzureAIService _azureAIService;

        public ResourcesFunctions(IResourcesService resourcesService, IBlobStorageService blobStorageService, IAzureAIService azureAIService)
        {
            _resourcesService = resourcesService;
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

                Resource newResource = await _resourcesService.CreateAsync(new Resource());
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

                string resourceData = await _azureAIService.GetResourceData(extractedText);
                newResource = JsonSerializer.Deserialize<Resource>(resourceData);
                newResource.Id = newResourceId;
                newResource.CVName = fileName;

                await _resourcesService.UpdateAsync(newResourceId, newResource);

                log.LogInformation("Resource Inserted");
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                log.LogError("Errore: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };
            }
        }
    }
}
