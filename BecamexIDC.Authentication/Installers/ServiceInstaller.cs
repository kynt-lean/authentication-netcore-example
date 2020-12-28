using BecamexIDC.Authentication.Services;
using BecamexIDC.Authentication.Services.ApiSmartInService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BecamexIDC.Authentication.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IFunctionApiService, FunctionApiService>();
            services.AddScoped<IBCMAppService, BCMAppService>();
            services.AddScoped<IFactoryApiService, FactoryApiService>();
            services.AddScoped<IUserFactoryService, UserFactoryService>();
          
        }
    }
}