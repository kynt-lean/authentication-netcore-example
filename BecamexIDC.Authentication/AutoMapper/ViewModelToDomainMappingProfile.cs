using AutoMapper;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Models.Entities;
using BecamexIDC.Authentication.Models.ViewModels;

namespace BecamexIDC.Authentication.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<PermissionViewModel,Permissions >();
            CreateMap<UserFactoryViewModel, UserFactory>();
        }
    }
}