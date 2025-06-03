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
using FinanceFunctions.Models.EmployeeModels;

namespace FinanceFunctions.Functions
{
    public class ProjectAssignmentsFunctions
    {
        private readonly IProjectAssignmentsService _assignmentsService;
        private readonly IEmployeesService _employeesService;
        private readonly IProjectsService _projectsService;
        private readonly IAzureAIService _azureAIService; // Per la funzionalità di matching

        public ProjectAssignmentsFunctions(IProjectAssignmentsService assignmentsService,  IAzureAIService azureAIService, IEmployeesService employeesService, IProjectsService projectsService)
        {
            _assignmentsService = assignmentsService;
            _azureAIService = azureAIService;
            _employeesService = employeesService;
            _projectsService = projectsService;
        }

        [FunctionName("CreateProjectAssignment")]
        public async Task<HttpResponseMessage> CreateProjectAssignment(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "assignments")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var assignmentInput = System.Text.Json.JsonSerializer.Deserialize<ProjectAssignment>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (assignmentInput == null || string.IsNullOrWhiteSpace(assignmentInput.EmployeeId) || string.IsNullOrWhiteSpace(assignmentInput.ProjectId))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("EmployeeId and ProjectId are required for assignment.")
                    };
                }

                await _assignmentsService.CreateAsync(assignmentInput);

                log.LogInformation($"Project assignment created for Employee {assignmentInput.EmployeeId} on Project {assignmentInput.ProjectId}.");
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(assignmentInput))
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error creating project assignment: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetProjectAssignments")]
        public async Task<HttpResponseMessage> GetProjectAssignments(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assignments")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var assignments = await _assignmentsService.GetAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(assignments), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting project assignments: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetProjectAssignmentById")]
        public async Task<HttpResponseMessage> GetProjectAssignmentById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assignments/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var assignment = await _assignmentsService.GetByIdAsync(id);
                if (assignment == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(assignment), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting project assignment by ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetAssignmentsByEmployee")]
        public async Task<HttpResponseMessage> GetAssignmentsByEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees/{employeeId}/assignments")] HttpRequest req,
            string employeeId,
            ILogger log)
        {
            try
            {
                var assignments = await _assignmentsService.GetByEmployeeIdAsync(employeeId);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(assignments), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting assignments for employee {employeeId}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetAssignmentsByProject")]
        public async Task<HttpResponseMessage> GetAssignmentsByProject(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "projects/{projectId}/assignments")] HttpRequest req,
            string projectId,
            ILogger log)
        {
            try
            {
                var assignments = await _assignmentsService.GetByProjectIdAsync(projectId);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(assignments), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting assignments for project {projectId}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("UpdateProjectAssignment")]
        public async Task<HttpResponseMessage> UpdateProjectAssignment(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "assignments/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var assignmentInput = System.Text.Json.JsonSerializer.Deserialize<ProjectAssignment>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await _assignmentsService.UpdateAsync(id, assignmentInput);

                log.LogInformation($"Project assignment with ID {id} updated successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating project assignment with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("DeleteProjectAssignment")]
        public async Task<HttpResponseMessage> DeleteProjectAssignment(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "assignments/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                bool deleted = await _assignmentsService.DeleteAsync(id);
                if (!deleted)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                log.LogInformation($"Project assignment with ID {id} deleted successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting project assignment with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("MatchEmployeesToProject")]
        public async Task<HttpResponseMessage> MatchEmployeesToProject(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "projects/{projectId}/match-employees")] HttpRequest req,
            string projectId,
            ILogger log)
        {
            try
            {
                // Recupera il progetto per le sue skill richieste
                var project = await _projectsService.GetByIdAsync(projectId);
                if (project == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent($"Project with ID {projectId} not found.") };
                }

                // Recupera tutti i dipendenti disponibili
                var allEmployees = await _employeesService.GetAsync();
                var availableEmployees = allEmployees.Where(e => e.IsAvailable).ToList();

                // Chiama il servizio AI per fare il matching
                // Assumiamo che GetBestMatchesForProject restituisca una lista di Employee con un punteggio di match
                // o un DTO specifico per il matching.
                var matchedEmployees = await _azureAIService.GetBestMatchesForProject(project, availableEmployees);
                //await _assignmentsService.CreateAsync(matchedEmployees);
                //var matchedEmployees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeMatchResult>>(matchResultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                log.LogInformation($"Matching completed for project {projectId}.");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(matchedEmployees), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error matching employees to project {projectId}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }
    }
}
