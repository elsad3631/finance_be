using FinanceFunctions.CosmosEntities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceFunctions.Models.EmployeeModels
{
    public class EmployeeMatchResult
    {
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }
        [JsonPropertyName("matchScore")]
        public double MatchScore { get; set; } // Punteggio di matching (es. 0.0 - 1.0)
        [JsonPropertyName("missingSkills")]
        public List<string> MissingSkills { get; set; } // Competenze mancanti per un match perfetto
        [JsonPropertyName("potentialSkills")]
        public List<string> PotentialSkills { get; set; } // Competenze extra che potrebbero essere utili
                                                          // Puoi aggiungere qui una versione semplificata dell'oggetto Employee se vuoi che i dati del dipendente siano restituiti direttamente
        [JsonPropertyName("employeeDetails")]
        public Employee EmployeeDetails { get; set; } // Dettagli del dipendente
    }
}
