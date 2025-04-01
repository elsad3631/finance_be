namespace BackEnd.Models.StartingInfos
{
    public class StartingInfosUpdateModel
    {
        public string Id { get; set; } = string.Empty;
        public string UseCase { get; set; }
        public string Experiace { get; set; }
        public string PlatformUsedToInvest { get; set; }
        public string InvestmentsTypes { get; set; }
        public string FinancialPurpose { get; set; }
        public string UpdateDate { get { return DateTime.Now.ToString(); } }
    }
}
