using StatisticalWorker.Service.BLL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticalWorker.Workers
{
    public class LeaderBoardWorker : BackgroundService
    {
        #region - property -
        private Timer timer;
        private readonly SetLeaderBoardTableBLL setLeaderBoardTableBLL;
        private readonly ILogger<LeaderBoardWorker> logger;
        #endregion

        #region - Construction method -
        public LeaderBoardWorker(ILogger<LeaderBoardWorker> logger, SetLeaderBoardTableBLL setLeaderBoardTableBLL)
        {
            this.logger = logger;
            this.setLeaderBoardTableBLL = setLeaderBoardTableBLL;
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
            double utc = ConvertDateTimeInt(utcNow);
            DateTime dtime = ConvertIntDatetime(utc);

            //幾分的時候執行,一小內執行兩次
            int first = 1;
            int second = 31;

            try
            {
                logger.LogInformation(string.Format("LeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));
                var LeaderBoardData = setLeaderBoardTableBLL.GetLeaderBoardData();              
                setLeaderBoardTableBLL.SetLeaderBoardTable(LeaderBoardData);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                //下一次還要多久分鐘後執行
                int time = 0;
                //現在幾分
                int minute = DateTime.Now.Minute;

                if(second < minute)
                {
                    time = 60 - minute + first;
                }
                else if(first > minute)
                {
                    time = first - minute;
                }
                else
                {
                    time = second - minute;
                }

                TimeSpan dueTimeSpan = TimeSpan.FromMinutes(minute);
                timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private int ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            intResult = (time - startTime).TotalSeconds;

            return (int)intResult;

        }

        private DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            startTime = startTime.AddSeconds(utc);

            startTime = startTime.AddHours(8);

            return startTime;

        }
        #endregion
    }
}
