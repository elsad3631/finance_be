using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.TransactionSummary;

namespace BackEnd.Profiles
{
    public class TransactionSummaryProfiles : Profile
    {
        public TransactionSummaryProfiles() 
        {
            CreateMap<TransactionSummary, TransactionSummaryCreateModel>();
            CreateMap<TransactionSummaryCreateModel, TransactionSummary>();

            CreateMap<TransactionSummary, TransactionSummaryUpdateModel>();
            CreateMap<TransactionSummaryUpdateModel, TransactionSummary>();
        }
    }
}
