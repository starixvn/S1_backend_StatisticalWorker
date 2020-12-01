using StatisticalWorker.Service.BLL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StatisticalWorker.Service.Library;

namespace StatisticalWorker.Workers
{
    public class LeaderBoardWorker : BackgroundService
    {
        #region - property -
        private Timer timer;
        private readonly SetLeaderBoardTableBLL setLeaderBoardTableBLL;
        private readonly ILogger<LeaderBoardWorker> logger;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public LeaderBoardWorker(ILogger<LeaderBoardWorker> logger, SetLeaderBoardTableBLL setLeaderBoardTableBLL, Log2File log2File)
        {
            this.logger = logger;
            this.setLeaderBoardTableBLL = setLeaderBoardTableBLL;
            this.log2File = log2File;
        }
        #endregion

        #region - protected method- 
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoLeaderBoard, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }
        #endregion

        #region - private method - 
        private void DoLeaderBoard(object state)
        {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("LeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "LeaderBoardWorkerLog");
                logger.LogInformation(string.Format("LeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var leaderBoardDataList = setLeaderBoardTableBLL.GetLeaderBoardData(0).ToList();

                var dd = leaderBoardDataList.ToDictionary(x => $"{x.LBCategoryId}_{x.CustId}", x => x);

                    foreach (var leaderBoardDataList14 in setLeaderBoardTableBLL.GetLeaderBoardData(1))
                    {
                        if (dd.ContainsKey($"{leaderBoardDataList14.LBCategoryId}_{leaderBoardDataList14.CustId}"))
                        {
                            dd[$"{leaderBoardDataList14.LBCategoryId}_{leaderBoardDataList14.CustId}"].Turnover += leaderBoardDataList14.Turnover;
                        }
                        else
                        {
                            leaderBoardDataList.Add(leaderBoardDataList14);
                        }
                    }
             

                setLeaderBoardTableBLL.SetLeaderBoardTable(leaderBoardDataList);
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message, "LeaderBoardWorkerLog");
                logger.LogError(ex.Message);
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
