using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Installers {
    public interface IInstaller {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}