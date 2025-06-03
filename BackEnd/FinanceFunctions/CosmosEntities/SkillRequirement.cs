using Newtonsoft.Json;

namespace FinanceFunctions.CosmosEntities
{
    // Per le competenze richieste da un progetto
    public class SkillRequirement
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("min_proficiency_level")]
        public int MinProficiencyLevel { get; set; } // Livello minimo richiesto

        [JsonProperty("is_mandatory")] // Indica se la skill è obbligatoria per il progetto
        public bool IsMandatory { get; set; }
    }
}
