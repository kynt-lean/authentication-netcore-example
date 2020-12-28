using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Jobs
{
    public class AddEOfficeAccountsJob : IJob
    {

        private readonly ILogger<AddEOfficeAccountsJob> _logger;
        private readonly IServiceProvider _provider;

        public AddEOfficeAccountsJob(ILogger<AddEOfficeAccountsJob> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                _logger.LogInformation("Run AddEOfficeUserInfoJob at {Time}:", DateTime.UtcNow.ToString());
                var _context = scope.ServiceProvider.GetService<AuthDbContext>();
                var _identityService = scope.ServiceProvider.GetService<IIdentityService>();
                List<EOfficeUserInfo> eOfficeAccounts = await _identityService.GetEOfficeAccountsSharePoint();
                _context.EOfficeUserInfo.RemoveRange(_context.EOfficeUserInfo.ToList());
                _context.EOfficeUserInfo.AddRange(eOfficeAccounts);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Complete AddEOfficeUserInfoJob at {Time}:", DateTime.UtcNow.ToString());
            }
        }
    }
}
