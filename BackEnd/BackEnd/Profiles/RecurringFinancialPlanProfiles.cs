using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.RecurringFinancialPlan;

namespace BackEnd.Profiles
{
    public class RecurringFinancialPlanProfiles : Profile
    {
        public RecurringFinancialPlanProfiles() 
        {
            CreateMap<RecurringFinancialPlan, RecurringFinancialPlanCreateModel>();
            CreateMap<RecurringFinancialPlanCreateModel, RecurringFinancialPlan>();

            CreateMap<RecurringFinancialPlan, RecurringFinancialPlanUpdateModel>();
            CreateMap<RecurringFinancialPlanUpdateModel, RecurringFinancialPlan>();
        }
    }
}
