using FinanceFunctions.CosmosEntities;
using System.Text.Json.Serialization;

namespace FinanceFunctions.Models
{
    public class BraveSearchSelectModel
    {
        [JsonPropertyName("news")]
        public NewsSection News { get; set; }
    }
}
