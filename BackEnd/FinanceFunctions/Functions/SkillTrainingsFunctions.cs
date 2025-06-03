using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinanceFunctions.Functions
{
    public class SkillTrainingsFunctions
    {
        private readonly ISkillTrainingsService _skillTrainingsService;
        private readonly IAzureAIService _azureAIService;
        private readonly IEmployeesService _employeesService; // Per suggerimenti di formazione

        public SkillTrainingsFunctions(ISkillTrainingsService skillTrainingsService, IEmployeesService employeesService, IAzureAIService azureAIService)
        {
            _skillTrainingsService = skillTrainingsService;
            _employeesService = employeesService;
            _azureAIService = azureAIService;
        }

        [FunctionName("CreateSkillTraining")]
        public async Task<HttpResponseMessage> CreateSkillTraining(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "skill-trainings")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var trainingInput = System.Text.Json.JsonSerializer.Deserialize<SkillTraining>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (trainingInput == null || string.IsNullOrWhiteSpace(trainingInput.Title))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Training title is required.")
                    };
                }

                await _skillTrainingsService.CreateAsync(trainingInput);

                log.LogInformation($"Skill training '{trainingInput.Title}' created successfully.");
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(trainingInput))
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error creating skill training: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetSkillTrainings")]
        public async Task<HttpResponseMessage> GetSkillTrainings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "skill-trainings")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var trainings = await _skillTrainingsService.GetAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(trainings), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting skill trainings: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetSkillTrainingById")]
        public async Task<HttpResponseMessage> GetSkillTrainingById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "skill-trainings/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var training = await _skillTrainingsService.GetByIdAsync(id);
                if (training == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(training), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting skill training by ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetSkillTrainingsBySkill")]
        public async Task<HttpResponseMessage> GetSkillTrainingsBySkill(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "skill-trainings/by-skill/{skillName}")] HttpRequest req,
            string skillName,
            ILogger log)
        {
            try
            {
                var trainings = await _skillTrainingsService.GetBySkillDevelopedAsync(skillName);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(trainings), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting skill trainings for skill '{skillName}': {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("UpdateSkillTraining")]
        public async Task<HttpResponseMessage> UpdateSkillTraining(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "skill-trainings/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var trainingInput = System.Text.Json.JsonSerializer.Deserialize<SkillTraining>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await _skillTrainingsService.UpdateAsync(id, trainingInput);

                log.LogInformation($"Skill training with ID {id} updated successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating skill training with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("DeleteSkillTraining")]
        public async Task<HttpResponseMessage> DeleteSkillTraining(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "skill-trainings/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                bool deleted = await _skillTrainingsService.DeleteAsync(id);
                if (!deleted)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                log.LogInformation($"Skill training with ID {id} deleted successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting skill training with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("SuggestTrainingsForEmployee")]
        public async Task<HttpResponseMessage> SuggestTrainingsForEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees/{employeeId}/suggested-trainings")] HttpRequest req,
            string employeeId,
            ILogger log)
        {
            try
            {
                var employee = await _employeesService.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent($"Employee with ID {employeeId} not found.") };
                }

                // Recupera tutti i corsi disponibili
                var allTrainings = await _skillTrainingsService.GetAsync();

                // Chiama il servizio AI per suggerire i corsi basati sulle skill dell'employee
                // e magari sulle skill gap identificate.
                // Assumiamo che GetSuggestedTrainings ritorni una lista di SkillTraining.
                var suggestedTrainings = await _azureAIService.GetSuggestedTrainings(employee, allTrainings);

                //var suggestedTrainings = System.Text.Json.JsonSerializer.Deserialize<List<SkillTraining>>(suggestedTrainingsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                log.LogInformation($"Suggested trainings for employee {employeeId} retrieved.");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(suggestedTrainings), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error suggesting trainings for employee {employeeId}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }
    }
}
