using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatisticalWorker.Service.BLL;
using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Workers;

namespace StatisticalWorker
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            //Library
            services.AddSingleton<Log2File>();

            //DAL
            services.AddSingleton<SetLeaderBoardTableService>();
            services.AddSingleton<OnlineCountService>();

            //BLL
            services.AddSingleton<SetLeaderBoardTableBLL>();
            services.AddSingleton<OnlineCountBLL>();

            //Worker
            services.AddHostedService<LeaderBoardWorker>();
            services.AddHostedService<OnlineCountWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
