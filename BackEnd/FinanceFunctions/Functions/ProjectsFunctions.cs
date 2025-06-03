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
using FinanceFunctions.Services;

namespace FinanceFunctions.Functions
{
    public class ProjectsFunctions
    {
        private readonly IProjectsService _projectsService;
        private readonly IAzureAIService _azureAIService;
        public ProjectsFunctions(IProjectsService projectsService, IAzureAIService azureAIService)
        {
            _projectsService = projectsService;
            _azureAIService = azureAIService;
        }

        [FunctionName("ExtractSkills")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "projects/extract-skills")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var jobInput = JsonSerializer.Deserialize<Project>(requestBody);

                if (string.IsNullOrWhiteSpace(jobInput?.Name) || string.IsNullOrWhiteSpace(jobInput?.Description))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Richiesti 'Titolo' e 'Descrizione' nel corpo della richiesta.")
                    };
                }

                var result = await _azureAIService.GetProjectSkills(jobInput.Name, jobInput.Description);

                //Project result = JsonSerializer.Deserialize<Project>(jobData);
                await _projectsService.CreateAsync(result);

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

        [FunctionName("ExtractProjectSkillsAndCreate")]
        public async Task<HttpResponseMessage> ExtractProjectSkillsAndCreate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "projects/extract-and-create")] HttpRequest req,
        ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var projectInput = System.Text.Json.JsonSerializer.Deserialize<Project>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (string.IsNullOrWhiteSpace(projectInput?.Name) || string.IsNullOrWhiteSpace(projectInput?.Description))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Required 'Name' and 'Description' in the request body.")
                    };
                }

                // Chiamata all'AI per estrarre le competenze
                // Assumiamo che GetProjectSkills ritorni un JSON che mappa ai RequiredHardSkills e RequiredSoftSkills
                var aiSkills = await _azureAIService.GetProjectSkills(projectInput.Name, projectInput.Description);

                // Deserializza la risposta dell'AI in un oggetto Project (o un DTO specifico per le skills)
                // e popola le liste di skills del projectInput
                //var aiSkills = System.Text.Json.JsonSerializer.Deserialize<Project>(aiResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                projectInput.RequiredHardSkills = aiSkills.RequiredHardSkills;
                projectInput.RequiredSoftSkills = aiSkills.RequiredSoftSkills;

                // Imposta lo stato iniziale del progetto
                projectInput.Status = "Draft"; // O "Open" a seconda della logica iniziale

                await _projectsService.CreateAsync(projectInput);

                log.LogInformation($"Project '{projectInput.Name}' created and skills extracted successfully.");
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(projectInput))
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ExtractProjectSkillsAndCreate: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetProjects")]
        public async Task<HttpResponseMessage> GetProjects(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "projects")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var projects = await _projectsService.GetAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(projects), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting projects: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetProjectById")]
        public async Task<HttpResponseMessage> GetProjectById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "projects/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var project = await _projectsService.GetByIdAsync(id);
                if (project == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(project), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting project by ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("UpdateProject")]
        public async Task<HttpResponseMessage> UpdateProject(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "projects/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var projectInput = System.Text.Json.JsonSerializer.Deserialize<Project>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await _projectsService.UpdateAsync(id, projectInput);

                log.LogInformation($"Project with ID {id} updated successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent); // 204 No Content for successful update
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating project with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("DeleteProject")]
        public async Task<HttpResponseMessage> DeleteProject(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "projects/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                bool deleted = await _projectsService.DeleteAsync(id);
                if (!deleted)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                log.LogInformation($"Project with ID {id} deleted successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting project with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }
    }
}
