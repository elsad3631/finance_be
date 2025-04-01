using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class RecurringTransaction : CosmosEntityBase
    {
        [JsonProperty("type")]
        public string Type { get; set; } // Income or Expense

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; } // Monthly, Annual, etc.

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }
    }
}
