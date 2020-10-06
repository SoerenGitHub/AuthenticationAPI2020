using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AuthAPI.Installers {
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

             services.AddSwaggerGen(x => {

                x.SwaggerDoc("v1", new OpenApiInfo{
                    Title = "Data API", 
                    Version = "v1"
                });

                OpenApiSecurityRequirement req = new OpenApiSecurityRequirement();
                

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Description = "JWT Authorazation header using bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(req);
            });
        }
    }
}