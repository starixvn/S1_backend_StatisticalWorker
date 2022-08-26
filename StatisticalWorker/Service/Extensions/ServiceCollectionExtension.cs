using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model.enums;

namespace StatisticalWorker.Service.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? config.GetValue<string>("ServiceType");

            return services
               .AddSingleton(param => new MySqlClient(config.GetConnectionString($"{serviceType}_MainDB"), logger, DataBase.Main))
               .AddSingleton(param => new MySqlClient(config.GetConnectionString($"{serviceType}_ReportDB"), logger, DataBase.Report));
        }
    }
}
