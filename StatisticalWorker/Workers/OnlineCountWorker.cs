using StatisticalWorker.Service.BLL;

namespace StatisticalWorker.Workers
{
    public class OnlineCountWorker : BackgroundService
    {
        private Timer timer;
        private readonly OnlineCountBLL _onlinePopulationBLL;
        private readonly ILogger<OnlineCountWorker> _logger;

        public OnlineCountWorker(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<OnlineCountWorker>>();
            _onlinePopulationBLL = serviceProvider.GetRequiredService<OnlineCountBLL>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(SetOnlineCount, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }

        private void SetOnlineCount(object state)
        {
            var dTime = GetUTC8Time();
            try
            {                
                _logger.LogInformation($"OnlineCountWorker-{dTime:yyyy/MM/dd HH:mm:ss}");
                var onlineCountData = _onlinePopulationBLL.GetOnlineOnlineUsersData();
                _onlinePopulationBLL.SetOnlineCountData(onlineCountData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                //執行完後，隔10分鐘再執行
                var dueTimeSpan = TimeSpan.FromMinutes(10);
                timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }

    }
}
