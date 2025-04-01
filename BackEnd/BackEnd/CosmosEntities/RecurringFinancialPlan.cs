using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class RecurringFinancialPlan : CosmosEntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; } // Monthly, Annual, etc.

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }
    }
}
