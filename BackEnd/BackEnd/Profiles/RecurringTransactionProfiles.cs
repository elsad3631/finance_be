using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.RecurringFinancialPlan;

namespace BackEnd.Profiles
{
    public class RecurringTransactionProfiles : Profile
    {
        public RecurringTransactionProfiles() 
        {
            CreateMap<RecurringFinancialPlan, RecurringFinancialPlanCreateModel>();
            CreateMap<RecurringFinancialPlanCreateModel, RecurringFinancialPlan>();

            CreateMap<RecurringFinancialPlan, RecurringFinancialPlanUpdateModel>();
            CreateMap<RecurringFinancialPlanUpdateModel, RecurringFinancialPlan>();
        }
    }
}
