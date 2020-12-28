using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BecamexIDC.Authentication.Options;
using System;

namespace BecamexIDC.Authentication.Installers
{
    public class LDAPInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var ldapSettings = new LDAPSettings();
            var wcfSettings = new WcfSettings(); 
            // ldapSettings from environment
            ldapSettings.Address = Environment.GetEnvironmentVariable("BCM_LDAP_ADDRESS");
            ldapSettings.Port = Environment.GetEnvironmentVariable("BCM_LDAP_PORT");
            ldapSettings.Account = Environment.GetEnvironmentVariable("BCM_LDAP_ACCOUNT");
            ldapSettings.Password = Environment.GetEnvironmentVariable("BCM_LDAP_PASSWORD");
            ldapSettings.Domain = Environment.GetEnvironmentVariable("BCM_LDAP_DOMAIN");
            // wcfSettings from environment
            wcfSettings.RemoteAddress = Environment.GetEnvironmentVariable("BCM_WCF_ADDRESS") + Environment.GetEnvironmentVariable("BCM_WCF_AUTHENTICATION");

            //configuration.Bind(nameof(ldapSettings), ldapSettings);
            //configuration.Bind(nameof(wcfSettings), wcfSettings);
            services.AddSingleton(ldapSettings);           
            services.AddSingleton(wcfSettings);
        }
    }
}