using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BecamexIDC.Authentication.Installers
{
    public interface IInstaller
    {
         void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}