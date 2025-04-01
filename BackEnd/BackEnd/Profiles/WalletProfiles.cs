using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Models.Wallet;

namespace BackEnd.Profiles
{
    public class WalletProfiles : Profile
    {
        public WalletProfiles() 
        {
            CreateMap<Wallet, WalletCreateModel>();
            CreateMap<WalletCreateModel, Wallet>();

            CreateMap<Wallet, WalletUpdateModel>();
            CreateMap<WalletUpdateModel, Wallet>();
        }
    }
}
