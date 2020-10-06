using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Installers {
    public static class InstallerExtensions {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration) {
             //Get all installers
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x => 
                typeof(IInstaller).IsAssignableFrom(x) &&
                !x.IsInterface &&
                !x.IsAbstract
            )
            .Select(Activator.CreateInstance)
            .Cast<IInstaller>()
            .ToList();

            //Run installers
            installers.ForEach(installer => {
                installer.InstallServices(services, configuration);
            });
        }
    }
}