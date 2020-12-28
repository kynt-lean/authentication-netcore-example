using AutoMapper;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Models.Entities;
using BecamexIDC.Authentication.Models.ViewModels;
using System;
using System.Linq;

namespace BecamexIDC.Authentication.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Permissions, PermissionViewModel>();
            CreateMap<UserFactory, UserFactoryViewModel>();
        }
    }
}