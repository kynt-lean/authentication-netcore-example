
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Services.ApiSmartInService
{
    public interface IFactoryApiService
    {
        Task<FactoryReply> GetAllFactory();
    }
    public class FactoryApiService :IFactoryApiService
    {
       
        private readonly FactoryGRPCService.FactoryGRPCServiceClient _factoryClient;
        

        public FactoryApiService(FactoryGRPCService.FactoryGRPCServiceClient factoryClient)
        {
            
            _factoryClient = factoryClient;
        }

        public async Task<FactoryReply> GetAllFactory()
        {
            var factoryRequest = new FactoryRequest { };
            var reply = await _factoryClient.GetAllFactoryAsync(factoryRequest);
            return await Task.FromResult(reply);
        }
    }
}
