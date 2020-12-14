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
        private Timer doBGLeaderBoard;
        private Timer setBGLeaderBoardDeposit;
        private Timer doN88LeaderBoard;
        private Timer setN88LeaderBoardDeposit;
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
            doN88LeaderBoard = new Timer(DoN88LeaderBoard, null, TimeSpan.Zero, TimeSpan.Zero);
            doBGLeaderBoard = new Timer(DoBGLeaderBoard, null, TimeSpan.Zero, TimeSpan.Zero);
            setN88LeaderBoardDeposit = new Timer(SetN88LeaderBoardDeposit, null, TimeSpan.Zero, TimeSpan.Zero);
            setBGLeaderBoardDeposit = new Timer(SetBGLeaderBoardDeposit, null, TimeSpan.Zero, TimeSpan.Zero);

            return Task.CompletedTask;
        }
        #endregion

        #region - private method - 
        private void DoN88LeaderBoard(object state)
        {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("N88LeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "N88LeaderBoardWorkerLog");
                logger.LogInformation(string.Format("N88LeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var leaderBoardDataList = setLeaderBoardTableBLL.GetN88LeaderBoardData(0).ToList();

                var dd = leaderBoardDataList.ToDictionary(x => $"{x.CategoryId}_{x.CustId}", x => x);

                    foreach (var leaderBoardDataList14 in setLeaderBoardTableBLL.GetN88LeaderBoardData(1))
                    {
                        if (dd.ContainsKey($"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"))
                        {
                            dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].Turnover += leaderBoardDataList14.Turnover;
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
                log2File.Info(ex.Message, "N88LeaderBoardWorkerLog");
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
                doN88LeaderBoard.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private void DoBGLeaderBoard(object state)
        {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("BGLeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "BGLeaderBoardWorkerLog");
                logger.LogInformation(string.Format("BGLeaderBoardWorker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var leaderBoardDataList = setLeaderBoardTableBLL.GetBGLeaderBoardData(0).ToList();

                var dd = leaderBoardDataList.ToDictionary(x => $"{x.CategoryId}_{x.CustId}", x => x);

                foreach (var leaderBoardDataList14 in setLeaderBoardTableBLL.GetBGLeaderBoardData(1))
                {
                    if (dd.ContainsKey($"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"))
                    {
                        dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].Turnover += leaderBoardDataList14.Turnover;
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
                log2File.Info(ex.Message, "BGLeaderBoardWorkerLog");
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
                doBGLeaderBoard.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private void SetN88LeaderBoardDeposit(object state)
         {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("SetN88LeaderBoardDeposit-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "SetN88LeaderBoardDepositLog");
                logger.LogInformation(string.Format("SetN88LeaderBoardDeposit-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var DepositList = setLeaderBoardTableBLL.GetN88LeaderBoardDeposit();
                setLeaderBoardTableBLL.SetLeaderBoardDeposit(DepositList);
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message, "SetN88LeaderBoardDepositLog");
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
                setN88LeaderBoardDeposit.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private void SetBGLeaderBoardDeposit(object state)
        {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("SetBGLeaderBoardDeposit-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "SetBGLeaderBoardDepositLog");
                logger.LogInformation(string.Format("SetBGLeaderBoardDeposit-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var DepositList = setLeaderBoardTableBLL.GetBGLeaderBoardDeposit();
                setLeaderBoardTableBLL.SetLeaderBoardDeposit(DepositList);
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message, "SetBGLeaderBoardDepositLog");
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
                setBGLeaderBoardDeposit.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }
        #endregion
    }
}
