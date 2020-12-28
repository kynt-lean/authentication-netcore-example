using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BecamexIDC.Authentication.Installers
{
    public class GrpcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // The following statement allows you to call insecure services. To be used only in development environments.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            string serverAddress = Environment.GetEnvironmentVariable("BCM_GRPC");
            services.AddGrpcClient<FunctionGRPCService.FunctionGRPCServiceClient>(options => options.Address = new Uri(serverAddress));
            services.AddGrpcClient<ModuleGRPCService.ModuleGRPCServiceClient>(options => options.Address = new Uri(serverAddress));
            services.AddGrpcClient<FactoryGRPCService.FactoryGRPCServiceClient>(options => options.Address = new Uri(serverAddress));
        }
    }
}