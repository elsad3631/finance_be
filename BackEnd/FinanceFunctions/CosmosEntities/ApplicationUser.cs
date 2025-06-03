using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class ApplicationUser : CosmosEntityBase
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")] // L'email come identificatore univoco
        public string Email { get; set; }

        [JsonProperty("password_hash")] // Hash della password (non la password in chiaro!)
        public string PasswordHash { get; set; }

        [JsonProperty("roles")] // Ruoli dell'utente (es. "Employee", "Manager", "Admin")
        public List<string> Roles { get; set; }

        [JsonProperty("employee_id")] // Collega l'utente all'entità Employee corrispondente
        public string EmployeeId { get; set; }
    }
}
