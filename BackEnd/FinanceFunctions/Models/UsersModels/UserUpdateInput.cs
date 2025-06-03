using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceFunctions.Models.UsersModels
{
    public class UserUpdateInput
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }
    }
}
