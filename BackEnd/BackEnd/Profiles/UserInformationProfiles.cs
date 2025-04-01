using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.StartingInfos;

namespace BackEnd.Profiles
{
    public class UserInformationProfiles: Profile
    {
        public UserInformationProfiles() 
        {
            CreateMap<StartingInfos, StartingInfosCreateModel>();
            CreateMap<StartingInfosCreateModel, StartingInfos>();

            CreateMap<StartingInfos, StartingInfosUpdateModel>();
            CreateMap<StartingInfosUpdateModel, StartingInfos>();
        }
    }
}
