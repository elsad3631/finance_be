using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class Wallet : CosmosEntityBase
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }
        
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        
        [JsonProperty("ticker")]
        public string Ticker { get; set; }
    }
}
