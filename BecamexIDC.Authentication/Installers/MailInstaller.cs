using System;
using BecamexIDC.Authentication.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace BecamexIDC.Authentication.Installers
{
    public class MailInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var mailSettings = new MailSettings();
            mailSettings.Server = Environment.GetEnvironmentVariable("BCM_MAIL_SERVER");
            mailSettings.Port = Int32.Parse(Environment.GetEnvironmentVariable("BCM_MAIL_PORT"));
            mailSettings.SenderName = Environment.GetEnvironmentVariable("BCM_MAIL_SENDERNAME");
            mailSettings.SenderEmail = Environment.GetEnvironmentVariable("BCM_MAIL_SENDEREMAIL");
            mailSettings.Account = Environment.GetEnvironmentVariable("BCM_MAIL_ACCOUNT");
            mailSettings.Password = Environment.GetEnvironmentVariable("BCM_MAIL_PASSWORD");

            //configuration.Bind(nameof(mailSettings), mailSettings);
            services.AddSingleton(mailSettings);
            services.AddMailKit(optionBuilder =>
             {
                 optionBuilder.UseMailKit(new MailKitOptions()
                 {
                     //get options from sercets.json
                     Server = mailSettings.Server,
                     Port = mailSettings.Port,
                     SenderName = mailSettings.SenderName,
                     SenderEmail = mailSettings.SenderEmail,

                     //can be optional with no authentication 
                     Account = mailSettings.Account,
                     Password = mailSettings.Password
                 });
             });
        }

    }
}