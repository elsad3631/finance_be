using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class TangibleAsset : CosmosEntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("purchase_date")]
        public DateTime PurchaseDate { get; set; }
    }
}
