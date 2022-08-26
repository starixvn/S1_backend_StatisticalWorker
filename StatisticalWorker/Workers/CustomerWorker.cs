using StatisticalWorker.Service.BLL;
using StatisticalWorker.Service.Extensions;

namespace StatisticalWorker.Workers
{
    public class CustomerWorker: BackgroundService
    {
        private Timer _timer;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<CustomerWorker> _logger;
        private readonly CustomerBLL _customerBll;

        public CustomerWorker(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CustomerWorker>>();
            _customerBll = serviceProvider.GetRequiredService<CustomerBLL>();
            _environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(SetCustomerStatus,
                null,
                TimeSpan.Zero,
                TimeSpan.Zero);

            return Task.CompletedTask;
        }

        private void SetCustomerStatus(object obj)
        {
            var success = true;
                    
            try
            {
                success = _customerBll.ProcessCustomerStatus();
            }
            catch (Exception ex)
            {
                success = false;
                _logger.LogError(ex, MethodExtension.GetCurrentMethod());
            }
            finally
            {
                var dueTimeSpan = TimeSpan.FromHours(4);
                if (_environment.IsDevelopment())
                {
                    dueTimeSpan = TimeSpan.FromMinutes(1);
                }

                if (!success)
                {
                    dueTimeSpan = TimeSpan.FromMinutes(10);
                }

                _timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}