using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class Project : CosmosEntityBase
    {
        [JsonProperty("name")] // Changed from Title to Name for consistency
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("required_hard_skills")] // Skills hard richieste
        public List<SkillRequirement> RequiredHardSkills { get; set; } // Vedere nuova entità SkillRequirement

        [JsonProperty("required_soft_skills")] // Soft skills richieste
        public List<string> RequiredSoftSkills { get; set; } // Oppure potremmo dettagliare con una nuova entità

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; } // Data di inizio prevista (nullable)

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; } // Data di fine prevista (nullable)

        [JsonProperty("status")] // Es. "Draft", "Open", "InProgress", "Completed", "Canceled"
        public string Status { get; set; }

        [JsonProperty("manager_id")] // ID del manager responsabile del progetto (se presente)
        public string ManagerId { get; set; }

        [JsonProperty("budget")] // Budget stimato (opzionale)
        public decimal? Budget { get; set; }

        [JsonProperty("priority")] // Priorità del progetto (es. "High", "Medium", "Low")
        public string Priority { get; set; }
    }
}
