using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;

namespace FinanceFunctions.Functions
{
    public class JobsFunctions
    {
        private readonly IJobsService _JobsService;
        private readonly IAzureAIService _azureAIService;
        public JobsFunctions(IJobsService JobsService, IAzureAIService azureAIService)
        {
            _JobsService = JobsService;
            _azureAIService = azureAIService;
        }

        [FunctionName("ExtractSkills")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Job/ExtractSkills")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var jobInput = JsonSerializer.Deserialize<Job>(requestBody);

                if (string.IsNullOrWhiteSpace(jobInput?.Title) || string.IsNullOrWhiteSpace(jobInput?.Description))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Richiesti 'Titolo' e 'Descrizione' nel corpo della richiesta.")
                    };
                }

                string jobData = await _azureAIService.GetJobData(jobInput.Title, jobInput.Description);

                Job result = JsonSerializer.Deserialize<Job>(jobData);
                await _JobsService.CreateAsync(result);

                log.LogInformation("Job Inserted");
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                log.LogError("Errore: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Errore interno: {ex.Message}")
                };
            }
        }
    }
}
