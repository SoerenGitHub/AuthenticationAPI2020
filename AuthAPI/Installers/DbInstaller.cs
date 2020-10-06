using AuthAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Installers {
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(opt => opt.UseMySql(
                configuration.GetConnectionString("DbConnection")
            ));
            services.AddScoped<IUserRepo, SqlUserRepo>();
            services.AddScoped<ITokenRepo, SqlTokenRepo>();
        }
    }
}