using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Models.ViewModels;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace BecamexIDC.Authentication.Services.ApiSmartInService
{
    public interface IFunctionApiService
    {
        Task<FunctionReply> GetAllFunction();
        Task<ModuleReply> GetAllModules();
    }
    public class FunctionApiService : IFunctionApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly FunctionGRPCService.FunctionGRPCServiceClient _functionClient;
        private readonly ModuleGRPCService.ModuleGRPCServiceClient _moduleClient;
        public FunctionApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration,
                                  FunctionGRPCService.FunctionGRPCServiceClient functionClient, ModuleGRPCService.ModuleGRPCServiceClient moduleClient)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _functionClient = functionClient;
            _moduleClient = moduleClient;
        }

        public async Task<FunctionReply> GetAllFunction()
        {
            var functionRequest = new FunctionRequest { };
            var reply = await _functionClient.GetAllFunctionAsync(functionRequest);
            return await Task.FromResult(reply);
        }

        public async Task<ModuleReply> GetAllModules()
        {
            var moduleRequest = new ModuleRequest { };
            var reply = await _moduleClient.GetAllModulesAsync(moduleRequest);
            return await Task.FromResult(reply);
        }
    }
}