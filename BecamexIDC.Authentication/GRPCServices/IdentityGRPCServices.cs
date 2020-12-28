using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static BecamexIDC.IndentityGRPCService;

namespace BecamexIDC.Authenticion.GRPCServices
{
    public class IdentityGRPCServices : IndentityGRPCServiceBase
    {
        private readonly ILogger<IdentityGRPCServices> _logger;
        private readonly IIdentityService _indentityService;
        public IdentityGRPCServices(ILogger<IdentityGRPCServices> logger, IIdentityService indentityService)
        {
            _logger = logger;
            _indentityService = indentityService;
        }

        public override async Task<res_GetUserNameById> GetUserNameById(req_GetUserNameById request, ServerCallContext context)
        {
            var result = new res_GetUserNameById();
            foreach (var user in request.User)
            {
                result.User.Add(new proto_User()
                {
                    Id = user.Id,
                    UserName = await _indentityService.GetEofficeUserNameById(user.Id)
                });
            }   
            return await Task.FromResult(result);
        }

    }
}