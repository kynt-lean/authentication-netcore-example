using BecamexIDC.Authentication.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BecamexIDC.Authentication.Installers
{
    public class PolicyInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //set policy
            services.AddAuthorization(options =>
            {
                //set for roles
                options.AddPolicy(RolePermissions.Dashboards.View, builder =>
                {
                    builder.RequireClaim(CustomClaimTypes.Permission, RolePermissions.Dashboards.View);
                });
                //set for user
                options.AddPolicy(UserPermission.Create, builder =>
               {
                   builder.RequireClaim(CustomClaimTypes.Permission, UserPermission.Create);
               });
                options.AddPolicy(UserPermission.View, builder =>
               {
                   builder.RequireClaim(CustomClaimTypes.Permission, UserPermission.View);
               });
            });
        }
    }
}