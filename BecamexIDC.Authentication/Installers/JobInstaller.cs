using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;
using BecamexIDC.Authentication.Jobs;

namespace BecamexIDC.Authentication.Installers
{
    public class JobInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {            
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            services.AddSingleton<AddEOfficeAccountsJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AddEOfficeAccountsJob),
                cronExpression: "0 0 6,12,18 * * ?")); //Every day at 6:00 12:00 18:00
                //cronExpression: "0 * * ? * *")); //Every minute
        }
    }
}