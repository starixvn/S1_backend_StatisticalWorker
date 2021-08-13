using StatisticalWorker.Service.BLL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;

namespace StatisticalWorker.Workers
{
    public class LeaderBoardWorker : BackgroundService
    {
        #region - property -
        private Timer doBGLeaderBoard;
        private Timer setBGLeaderBoardDeposit;
        private Timer doN88LeaderBoard;
        private Timer setN88LeaderBoardDeposit;
        private Timer doLeaderBoardV2;
        private readonly SetLeaderBoardTableBLL setLeaderBoardTableBLL;
        private readonly ILogger<LeaderBoardWorker> logger;
        private readonly Log2File log2File;
        private readonly IHostEnvironment environment;
        #endregion

        #region - Construction method -
        public LeaderBoardWorker(ILogger<LeaderBoardWorker> logger, SetLeaderBoardTableBLL setLeaderBoardTableBLL, IHostEnvironment environment, Log2File log2File)
        {
            this.logger = logger;
            this.setLeaderBoardTableBLL = setLeaderBoardTableBLL;
            this.log2File = log2File;
            this.environment = environment;
        }
        #endregion

        #region - protected method- 
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //doN88LeaderBoard = new Timer(DoN88LeaderBoard, null, TimeSpan.Zero, TimeSpan.Zero);
            //doBGLeaderBoard = new Timer(DoBGLeaderBoard, null, TimeSpan.Zero, TimeSpan.Zero);
           // setN88LeaderBoardDeposit = new Timer(SetN88LeaderBoardDeposit, null, TimeSpan.Zero, TimeSpan.Zero);
           // setBGLeaderBoardDeposit = new Timer(SetBGLeaderBoardDeposit, null, TimeSpan.Zero, TimeSpan.Zero);
            doLeaderBoardV2 = new Timer(DoLeaderBoardV2, null, TimeSpan.Zero, TimeSpan.Zero);

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
                        dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].WinlostAmount += leaderBoardDataList14.WinlostAmount;
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
                log2File.Error(ex.ToString(), "N88LeaderBoardWorkerLog");
                logger.LogError(ex, ex.Message);
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
                        dd[$"{leaderBoardDataList14.CategoryId}_{leaderBoardDataList14.CustId}"].WinlostAmount += leaderBoardDataList14.WinlostAmount;
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
                logger.LogError(ex, ex.Message);
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
                logger.LogError(ex, ex.Message);
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
                logger.LogError(ex, ex.Message);
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
        void LeaderBoardstatistics(LeaderBoardEvent EventData)
        {

            var leaderBoardDataList = setLeaderBoardTableBLL.GetLeaderBoardDataV2(0, EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.SPName).ToList();


            var dd = leaderBoardDataList.ToDictionary(x => $"{x.CategoryId}_{x.CustId}", x => x);

            foreach (var leaderBoardDataList14 in setLeaderBoardTableBLL.GetLeaderBoardDataV2(1, EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.SPName))
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
            foreach (var leaderBoardExtraPoint in setLeaderBoardTableBLL.GetLeaderBoardExtraPoint(EventData.SiteId, EventData.EventCode, EventData.StartTime, EventData.EndTime, EventData.TimeZoom, EventData.ExtraPointSP))
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

            setLeaderBoardTableBLL.SetLeaderBoardTableV2(leaderBoardDataList);

     }
        private void DoLeaderBoardV2(object state)
        {
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime dtime = GetUTC8Time();

            try
            {
                log2File.Info(string.Format("LeaderBoardV2Worker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")), "LeaderBoardV2WorkerLog");
                logger.LogInformation(string.Format("LeaderBoardV2Worker-{0}", dtime.ToString("yyyy/MM/dd HH:mm:ss")));

                var EventList = setLeaderBoardTableBLL.GetLeaderboardEvent();
                foreach (var data in EventList)
                {
                    logger.LogInformation($"SiteId:{data.SiteId}-EventCode:{data.EventCode} - {dtime.ToString("yyyy/MM/dd HH:mm:ss")}");

                    // log2File.Info($"SiteId:{data.SiteId}-{data.EventCode} - {dtime.ToString("yyyy/MM/dd HH:mm:ss")}","LeaderBoardV2WorkerLog");
                    LeaderBoardstatistics(data);
                    if (environment.IsDevelopment())
                    {
                        logger.LogInformation($"END SiteId:{data.SiteId}-EventCode:{data.EventCode} - {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");
                    }
                }
                setLeaderBoardTableBLL.CheckLeaderboardOverTime();
            }
            catch (Exception ex)
            {
                log2File.Error(ex.ToString(), "LeaderBoardV2WorkerLog");
                logger.LogError(ex, ex.Message);
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
                if (environment.IsDevelopment())
                {
                    logger.LogInformation(string.Format("finish LeaderBoardV2Worker-{0}", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")));
                }
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //  Console.WriteLine("StopAsync(CancellationToken cancellationToken)");
            //WriteToFile("WriteToFileHostedService: Process Stopped");
            //timerResetUsage?.Change(Timeout.Infinite, 0);

            doBGLeaderBoard?.Change(Timeout.Infinite, 0);
            setBGLeaderBoardDeposit?.Change(Timeout.Infinite, 0);
            doN88LeaderBoard?.Change(Timeout.Infinite, 0);
            setN88LeaderBoardDeposit?.Change(Timeout.Infinite, 0);
            doLeaderBoardV2?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        #endregion
    }
}
