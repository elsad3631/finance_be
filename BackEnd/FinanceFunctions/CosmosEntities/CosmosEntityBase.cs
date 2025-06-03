using Newtonsoft.Json;
using System;

namespace FinanceFunctions.CosmosEntities
{
    public class CosmosEntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    }
}
