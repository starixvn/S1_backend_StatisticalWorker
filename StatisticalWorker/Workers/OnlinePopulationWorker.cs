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
    public class OnlinePopulationWorker : BackgroundService
    {
        #region - property -
        private Timer timer;
        private readonly OnlinePopulationBLL onlinePopulationBLL;
        private readonly ILogger<LeaderBoardWorker> logger;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public OnlinePopulationWorker(ILogger<LeaderBoardWorker> logger, OnlinePopulationBLL onlinePopulationBLL, Log2File log2File)
        {
            this.logger = logger;
            this.onlinePopulationBLL = onlinePopulationBLL;
            this.log2File = log2File;
        }
        #endregion

        #region - protected method- 
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(SetOnlinePopulation, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }
        #endregion

        #region - private method - 
        private void SetOnlinePopulation(object state)
        {
            DateTime dtime = GetUTC8Time();
            try
            {
                log2File.Info(string.Format("OnlinePopulationWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "OnlinePopulationWorkerLog");
                logger.LogInformation(string.Format("OnlinePopulationWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));
                var onlinePopulationData = onlinePopulationBLL.GetOnlinePopulationData(dtime);
                foreach(var x in onlinePopulationData)
                {
                    x.CreateTime = dtime;
                }
                onlinePopulationBLL.SetOnlinePopulationData(onlinePopulationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                //執行完後，隔10分鐘再執行
                TimeSpan dueTimeSpan = TimeSpan.FromMinutes(10);
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
