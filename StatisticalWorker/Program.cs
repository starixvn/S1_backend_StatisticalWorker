using Serilog;
using Serilog.Exceptions;
using StatisticalWorker;
using StatisticalWorker.Service.Extensions;
using StatisticalWorker.Workers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddRemoteJsonConfig(builder.Configuration.GetSection("ConfigService").Get<string[]>());

var excludesMessage = builder.Configuration.GetSection("ExcludesMessage").Get<string[]>();
var excludesApiEndpoints = builder.Configuration.GetSection("ExcludesApiEndpoint").Get<string[]>();
builder.Host.UseSerilog((context, Configuration) =>
{
    Configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Filter.ByExcluding(c =>
            c.Properties.Any(p =>
                excludesApiEndpoints.Any(item => p.Value.ToString().Contains(item))
                || excludesMessage.Any(item => c.MessageTemplate.ToString().Contains(item))))
        .Enrich.WithExceptionDetails();
});

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);


builder.Services
    .AddHostedService<LeaderBoardWorker>()
    .AddHostedService<OnlineCountWorker>()
    .AddHostedService<TpBetRecordRelayForReportWorker>()
    .AddHostedService<CustClosingBalanceWorker>();

if (builder.Configuration.GetSection("RunCustomerWorker").Get<bool>())
{
    builder.Services.AddHostedService<CustomerWorker>();
}

var app = builder.Build();
startup.Configure(app, builder.Environment);

app.Run();