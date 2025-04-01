using Newtonsoft.Json;

namespace BackEnd.CosmosEntities
{
    public class TransactionSummary : CosmosEntityBase
    {
        [JsonProperty("type")]
        public string Type { get; set; } // Income or Expense

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_date")]
        public DateTime TransactionDate { get; set; }
    }
}
