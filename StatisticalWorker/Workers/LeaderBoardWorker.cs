using StatisticalWorker.Service.BLL;
using StatisticalWorker.Service.Model;

namespace StatisticalWorker.Workers
{
    public class LeaderBoardWorker : BackgroundService
    {
        private Timer doLeaderBoardV2;
        private readonly SetLeaderBoardTableBLL _setLeaderBoardTableBLL;
        private readonly ILogger<LeaderBoardWorker> _logger;
        private readonly IHostEnvironment _environment;

        public LeaderBoardWorker(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<LeaderBoardWorker>>();
            _setLeaderBoardTableBLL = provider.GetRequiredService<SetLeaderBoardTableBLL>();
            _environment = provider.GetRequiredService<IHostEnvironment>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            doLeaderBoardV2 = new Timer(DoLeaderBoardV2, null, TimeSpan.Zero, TimeSpan.Zero);

            return Task.CompletedTask;
        }

        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }

        void LeaderBoardstatistics(LeaderBoardEvent EventData)
        {

            var leaderBoardDataList = _setLeaderBoardTableBLL.GetLeaderBoardDataV2(0, EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.SPName).ToList();


            var dd = leaderBoardDataList.ToDictionary(x => $"{x.CategoryId}_{x.CustId}", x => x);

            foreach (var leaderBoardDataList14 in _setLeaderBoardTableBLL.GetLeaderBoardDataV2(1, EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.SPName))
            {
                if (dd.ContainsKey($"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"))
                {
                    dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].Turnover += leaderBoardDataList14.Turnover;
                    dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].WinlostAmount += leaderBoardDataList14.WinlostAmount;
                }
                else
                {
                    leaderBoardDataList.Add(leaderBoardDataList14);
                }
            }
            //var item = setLeaderBoardTableBLL.GetLeaderBoardExtraPoint(EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom);
            foreach (var leaderBoardExtraPoint in _setLeaderBoardTableBLL.GetLeaderBoardExtraPoint(EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.ExtraPointSP))
            {
                if (dd.ContainsKey($"{leaderBoardExtraPoint.CategoryId}_{leaderBoardExtraPoint.CustId}"))
                {
                    dd[$"{leaderBoardExtraPoint.CategoryId}_{leaderBoardExtraPoint.CustId}"].Turnover += leaderBoardExtraPoint.Turnover;
                    dd[$"{leaderBoardExtraPoint.CategoryId}_{leaderBoardExtraPoint.CustId}"].WinlostAmount += leaderBoardExtraPoint.WinlostAmount;
                }
                else
                {
                    leaderBoardDataList.Add(leaderBoardExtraPoint);
                }
            }

            _setLeaderBoardTableBLL.SetLeaderBoardTableV2(leaderBoardDataList);

     }

        private void DoLeaderBoardV2(object state)
        {
            DateTime dtime = GetUTC8Time();

            try
            {
                _logger.LogInformation(string.Format("LeaderBoardV2Worker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var EventList = _setLeaderBoardTableBLL.GetLeaderboardEvent();
                foreach (var data in EventList)
                {
                    _logger.LogInformation($"SiteId:{data.SiteId}-EventCode:{data.EventCode} - {dtime.ToString("yyyy/MM/dd HH:mm:ss")}");
            
                    LeaderBoardstatistics(data);
                    if (_environment.IsDevelopment())
                    {
                        _logger.LogInformation($"END SiteId:{data.SiteId}-EventCode:{data.EventCode} - {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");
                    }
                }
                _setLeaderBoardTableBLL.CheckLeaderboardOverTime();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                //下一次還要多久分鐘後執行
                int nextTime = 60;
                int nowMinute = GetUTC8Time().Minute;

                nextTime = nextTime - nowMinute + 1;
                if (nextTime > 30)
                    nextTime -= 30;
                TimeSpan dueTimeSpan = TimeSpan.FromMinutes(nextTime);
                doLeaderBoardV2.Change(dueTimeSpan, TimeSpan.Zero);
                if (_environment.IsDevelopment())
                {
                    _logger.LogInformation(string.Format("finish LeaderBoardV2Worker-{0}", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")));
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            doLeaderBoardV2?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

    }
}
