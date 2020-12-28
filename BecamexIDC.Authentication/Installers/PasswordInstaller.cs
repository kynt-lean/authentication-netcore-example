using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BecamexIDC.Authentication.Options;

namespace BecamexIDC.Authentication.Installers
{
    public class PasswordInstalller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var passwordSettings = new PasswordSettings();
            var lockoutSettings = new LockoutSettings();
            var userSettings = new UserSettings();
            configuration.Bind(nameof(passwordSettings), passwordSettings);
            configuration.Bind(nameof(lockoutSettings), lockoutSettings);
            configuration.Bind(nameof(userSettings), userSettings);

            services.AddSingleton(passwordSettings);
            services.AddSingleton(lockoutSettings);
            services.AddSingleton(userSettings);         

            services.Configure<IdentityOptions>(options =>
                      {
                          // Password settings
                          options.Password.RequireDigit = passwordSettings.RequireDigit;
                          options.Password.RequiredLength = passwordSettings.RequiredLength;
                          options.Password.RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
                          options.Password.RequireUppercase = passwordSettings.RequireUppercase;
                          options.Password.RequireLowercase = passwordSettings.RequireLowercase;
                          options.Password.RequiredUniqueChars = passwordSettings.RequiredUniqueChars;

                          // Lockout settings
                          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutSettings.DefaultLockoutTimeSpanInMins);
                          options.Lockout.MaxFailedAccessAttempts = lockoutSettings.MaxFailedAccessAttempts;
                          options.Lockout.AllowedForNewUsers = lockoutSettings.AllowedForNewUsers;

                          // User settings
                          options.User.RequireUniqueEmail = userSettings.RequireUniqueEmail;
                      });
        }
    }
}