
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BecamexIDC.Authentication.Data;
using BecamexIDC.DataExtension;
using AutoMapper;
using BecamexIDC.Authentication.AutoMapper;
using System;

namespace BecamexIDC.Authentication.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            string connectSharePoint = configuration.GetConnectionString(Environment.GetEnvironmentVariable("BCM_AUTH_SHAREPOINT_CONNECTION"));
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("BCM_AUTH_CONNECTION")));
            services.AddDbContext<BCMAppDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("BCM_MOBILE_APP_CONNECTION")));
            services.AddTransient<IQueryExtension>(s => new QueryExtension(connectSharePoint));
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddRoles<IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<AuthDbContext>();
           //Auto Mapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IMapper>(sp =>
            {
                return new Mapper(AutoMapperConfig.RegisterMappings());
            });
            
            services.AddSingleton(AutoMapperConfig.RegisterMappings());
        }
    }
}