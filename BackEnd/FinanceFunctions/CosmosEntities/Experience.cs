using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class Experience
    {
        [JsonProperty("job_title")] // Changed from title to job_title for clarity
        public string JobTitle { get; set; }

        [JsonProperty("company_name")] // Changed from agency to company_name
        public string CompanyName { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; } // Nullable per esperienza corrente

        [JsonProperty("description")] // Descrizione del ruolo e delle responsabilità
        public string Description { get; set; }

        [JsonProperty("technologies_used")] // Elenco di tecnologie o strumenti usati in questa esperienza
        public List<string> TechnologiesUsed { get; set; }
    }
}
