using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using BecamexIDC.Authentication.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using BecamexIDC.Authentication.Domain;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace BecamexIDC.Authentication.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("smartin_api", c =>
              {

                // access the DI container
                var serviceProvider = services.BuildServiceProvider();
                // Find the HttpContextAccessor service
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                // Get the bearer token from the request context (header)
                var bearerToken = httpContextAccessor.HttpContext.Request
                                      .Headers["Authorization"]
                                      .FirstOrDefault(h => h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));

                // Add authorization if found
                if (bearerToken != null)
                      c.DefaultRequestHeaders.Add("Authorization", bearerToken);

              }
            );

            services.AddMvc(option => option.EnableEndpointRouting = false)
                     .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                     .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                     .AddNewtonsoftJson(opt => opt.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WaterTreatment Api", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0]}
                };
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}