using System;
using System.Text;
using AuthAPI.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Installers {
    public class PolicyInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("isOwner",
                    policy => policy.Requirements.Add(new OwnerRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, OwnerDiscoverHandler>();
        }
    }
}