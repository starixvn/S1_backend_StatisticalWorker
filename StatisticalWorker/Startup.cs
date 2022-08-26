using StatisticalWorker.Service.BLL;
using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Extensions;
using StatisticalWorker.Service.Library;

namespace StatisticalWorker
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseConnection(_configuration);

            //DAL
            services.AddSingleton<SetLeaderBoardTableService>();
            services.AddSingleton<OnlineCountService>();
            services.AddSingleton<CustClosingBalanceService>();
            services.AddSingleton<CustomerService>();
            
            //BLL
            services.AddSingleton<SelLangBLL>();
            services.AddSingleton<SetLeaderBoardTableBLL>();
            services.AddSingleton<OnlineCountBLL>();
            services.AddSingleton<CustClosingBalanceBLL>();
            services.AddSingleton<CustomerBLL>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"Hello World ! This is Statistical Worker.\n" +
                                                      $"Environment = {env.EnvironmentName}.\n" +
                                                      $"Time = {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff' 'zzz''")}.");
                });
            });
        }
    }
}
