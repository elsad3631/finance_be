using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.TangibleAsset;

namespace BackEnd.Profiles
{
    public class TangibleAssetProfiles : Profile
    {
        public TangibleAssetProfiles() 
        {
            CreateMap<TangibleAsset, TangibleAssetCreateModel>();
            CreateMap<TangibleAssetCreateModel, TangibleAsset>();

            CreateMap<TangibleAsset, TangibleAssetUpdateModel>();
            CreateMap<TangibleAssetUpdateModel, TangibleAsset>();
        }
    }
}
