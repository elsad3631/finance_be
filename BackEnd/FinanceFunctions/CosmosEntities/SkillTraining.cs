using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class SkillTraining : CosmosEntityBase
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("provider")] // Chi offre il corso (es. Coursera, Udemy, Internal Training)
        public string Provider { get; set; }

        [JsonProperty("url")] // URL al corso
        public string Url { get; set; }

        [JsonProperty("estimated_duration_hours")]
        public int EstimatedDurationHours { get; set; }

        [JsonProperty("skills_developed")] // Competenze che il corso aiuta a sviluppare
        public List<string> SkillsDeveloped { get; set; }

        [JsonProperty("level")] // Livello del corso (es. "Beginner", "Intermediate", "Advanced")
        public string Level { get; set; }

        [JsonProperty("cost")] // Costo del corso (opzionale)
        public decimal? Cost { get; set; }
    }
}
