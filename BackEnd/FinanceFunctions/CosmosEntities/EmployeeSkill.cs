using Newtonsoft.Json;
using System;

namespace FinanceFunctions.CosmosEntities
{
    // Per le competenze possedute da un dipendente
    public class EmployeeSkill
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proficiency_level")] // Changed from vote to proficiency_level for clarity
        public int? ProficiencyLevel { get; set; } // Es. Scala da 1 a 5 (base a esperto)

        [JsonProperty("last_used")] // Ultima volta che la skill è stata utilizzata (utile per l'AI)
        public DateTime? LastUsed { get; set; }

        [JsonProperty("certification")] // Se presente una certificazione
        public string Certification { get; set; }
    }
}
