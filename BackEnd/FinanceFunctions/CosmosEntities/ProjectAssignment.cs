using Newtonsoft.Json;
using System;

namespace FinanceFunctions.CosmosEntities
{
    public class ProjectAssignment : CosmosEntityBase
    {
        [JsonProperty("employee_id")]
        public string EmployeeId { get; set; }

        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("role_on_project")] // Ruolo del dipendente all'interno di questo specifico progetto
        public string RoleOnProject { get; set; }

        [JsonProperty("assignment_start_date")]
        public DateTime AssignmentStartDate { get; set; }

        [JsonProperty("assignment_end_date")]
        public DateTime? AssignmentEndDate { get; set; } // Nullable se il progetto è ancora in corso

        [JsonProperty("allocation_percentage")] // Percentuale di tempo allocato al progetto (es. 100% full-time, 50% part-time)
        public int AllocationPercentage { get; set; }

        [JsonProperty("status")] // Stato dell'assegnazione (es. "Proposed", "Assigned", "Completed", "Removed")
        public string Status { get; set; }

        [JsonProperty("feedback_received")] // Eventuale feedback sul dipendente per questo progetto
        public string FeedbackReceived { get; set; }
    }
}
