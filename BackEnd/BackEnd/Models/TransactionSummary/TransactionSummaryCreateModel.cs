namespace BackEnd.Models.TransactionSummary
{
    public class TransactionSummaryCreateModel
    {
        public string UseCase { get; set; } = string.Empty;
        public string Experiace { get; set; } = string.Empty;
        public string PlatformUsedToInvest { get; set; } = string.Empty;
        public string InvestmentsTypes { get; set; } = string.Empty;
        public string FinancialPurpose { get; set; } = string.Empty;
        public string CreationDate { get { return DateTime.Now.ToString(); } }
    }
}
