using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatisticalWorker.Service.BLL;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticalWorker.Workers
{
    public class OnlineCountWorker : BackgroundService
    {
        #region - property -
        private Timer timer;
        private readonly OnlineCountBLL onlinePopulationBLL;
        private readonly ILogger<OnlineCountWorker> logger;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public OnlineCountWorker(ILogger<OnlineCountWorker> logger, OnlineCountBLL onlinePopulationBLL, Log2File log2File)
        {
            this.logger = logger;
            this.onlinePopulationBLL = onlinePopulationBLL;
            this.log2File = log2File;
        }
        #endregion

        #region - protected method- 
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(SetOnlineCount, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }
        #endregion

        #region - private method - 
        private void SetOnlineCount(object state)
        {
            var dTime = GetUTC8Time();
            try
            {
                log2File.Info(string.Format("OnlineCountWorker-{0}", dTime.ToString("yyyy/MM/dd HH:mm:ss")), "OnlineCountWorker");
                logger.LogInformation(string.Format("OnlineCountWorker-{0}", dTime.ToString("yyyy/MM/dd HH:mm:ss")));
                var onlineCountData = onlinePopulationBLL.GetOnlineOnlineUsersData();
                onlinePopulationBLL.SetOnlineCountData(onlineCountData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
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
        #endregion
    }
}
