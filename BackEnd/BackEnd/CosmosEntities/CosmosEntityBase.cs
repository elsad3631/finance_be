using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class CosmosEntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("creation_date")]
        public string CreationDate { get; set; } = string.Empty;

        [JsonProperty("update_date")]
        public string UpdateDate { get; set; } = string.Empty;
    }
}
