using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class StartingInfos : CosmosEntityBase
    {
        [JsonProperty("use_case")]
        public string UseCase { get; set; }
        [JsonProperty("experiace")]
        public string Experiace { get; set; }
        [JsonProperty("platform_used_to_invest")]
        public string PlatformUsedToInvest { get; set; }
        [JsonProperty("investments_types")]
        public string InvestmentsTypes { get; set; }
        [JsonProperty("financial_purpose")]
        public string FinancialPurpose { get; set; }
    }
}
