using FinanceFunctions.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
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
using FinanceFunctions.Services;
using FinanceFunctions.Models.UsersModels;
using FinanceFunctions.CosmosEntities;

namespace FinanceFunctions.Functions
{
    public class UsersFunctions
    {
        private readonly ApplicationUsersService _usersService;
        // Potrebbe essere necessario un servizio di autenticazione/hashing password
        // private readonly IAuthService _authService; 

        public UsersFunctions(ApplicationUsersService usersService /*, IAuthService authService*/)
        {
            _usersService = usersService;
            //_authService = authService;
        }

        [FunctionName("CreateUser")]
        public async Task<HttpResponseMessage> CreateUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userInput = System.Text.Json.JsonSerializer.Deserialize<UserCreationInput>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userInput == null || string.IsNullOrWhiteSpace(userInput.Username) || string.IsNullOrWhiteSpace(userInput.Email) || string.IsNullOrWhiteSpace(userInput.Password))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Username, Email, and Password are required.")
                    };
                }

                // Verifica se l'utente esiste già
                var existingUser = await _usersService.GetByEmailAsync(userInput.Email);
                if (existingUser != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent($"User with email '{userInput.Email}' already exists.")
                    };
                }

                // Hashing della password (ESSENZIALE PER LA SICUREZZA)
                string hashedPassword = /*_authService.HashPassword(userInput.Password)*/ "hashed_password_placeholder"; // Sostituisci con la tua logica di hashing

                var newUser = new ApplicationUser
                {
                    Username = userInput.Username,
                    Email = userInput.Email,
                    PasswordHash = hashedPassword,
                    Roles = userInput.Roles ?? new List<string> { "Employee" }, // Ruolo di default se non specificato
                    EmployeeId = userInput.EmployeeId // Collega all'entità Employee se applicabile
                };

                await _usersService.CreateAsync(newUser);

                log.LogInformation($"User '{newUser.Username}' created successfully.");
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { newUser.Id, newUser.Username, newUser.Email, newUser.Roles }))
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error creating user: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetUsers")]
        public async Task<HttpResponseMessage> GetUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var users = await _usersService.GetAsync();
                // Non esporre gli hash delle password!
                var safeUsers = users.Select(u => new { u.Id, u.Username, u.Email, u.Roles, u.EmployeeId, u.CreationDate, u.UpdateDate });
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(safeUsers), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting users: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("GetUserById")]
        public async Task<HttpResponseMessage> GetUserById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(id);
                if (user == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                // Non esporre gli hash delle password!
                var safeUser = new { user.Id, user.Username, user.Email, user.Roles, user.EmployeeId, user.CreationDate, user.UpdateDate };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(safeUser), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting user by ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("UpdateUser")]
        public async Task<HttpResponseMessage> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "users/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userInput = System.Text.Json.JsonSerializer.Deserialize<UserUpdateInput>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var existingUser = await _usersService.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                // Aggiorna solo i campi consentiti. Non aggiornare la password direttamente qui.
                existingUser.Username = userInput.Username ?? existingUser.Username;
                existingUser.Email = userInput.Email ?? existingUser.Email;
                existingUser.Roles = userInput.Roles ?? existingUser.Roles;
                existingUser.EmployeeId = userInput.EmployeeId ?? existingUser.EmployeeId;

                await _usersService.UpdateAsync(id, existingUser);

                log.LogInformation($"User with ID {id} updated successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating user with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }

        [FunctionName("DeleteUser")]
        public async Task<HttpResponseMessage> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                bool deleted = await _usersService.DeleteAsync(id);
                if (!deleted)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                log.LogInformation($"User with ID {id} deleted successfully.");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error deleting user with ID {id}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal server error: {ex.Message}")
                };
            }
        }
    }
}
