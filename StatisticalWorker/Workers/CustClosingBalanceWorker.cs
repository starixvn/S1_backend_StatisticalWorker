using Microsoft.Extensions.Configuration;
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
    public class CustClosingBalanceWorker : BackgroundService
    {

        #region property

        private Timer _timer;

        private readonly ILogger<CustClosingBalanceWorker> _logger;
        private readonly Log2File _log2File;
        private readonly CustClosingBalanceBLL _custClosingBalanceBLL;
        private readonly IConfiguration _configuration;

        private string processUTCSevenDate = string.Empty;
        private string processUTCEightDate = string.Empty;
        #endregion

        #region Construction method

        public CustClosingBalanceWorker(ILogger<CustClosingBalanceWorker> logger,
                                               Log2File log2File,
                                               CustClosingBalanceBLL custClosingBalanceBLL,
                                               IConfiguration configuration)
        {
            _logger = logger;
            _log2File = log2File;
            _custClosingBalanceBLL = custClosingBalanceBLL;
            _configuration = configuration;
        }

        #endregion

        #region protected method 

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(SetCustClosingBalance,
                         null,
                               TimeSpan.Zero,
                               TimeSpan.Zero);

            return Task.CompletedTask;
        }

        #endregion

        #region private method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void SetCustClosingBalance(object obj)
        {

            DateTime? beginAt = null;
            string today = string.Empty;
            string runTime = GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss");

            int? timeZone = null;
            int coldDownTime = 10;
            int checkTime = 10;

            string startTime = string.Empty;
            string endTime = string.Empty;
            bool flag = false;

            try
            {
                _log2File.Info($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime}.", "CustClosingBalanceWorker");
                _logger.LogInformation($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime}.");

                #region CheckParameter

                for (int i = 7; i <= 8 && flag==false ; i++)
                {
                    switch (i)
                    {
                        case 7:
                            //UTC+7
                            if (i == _configuration.GetValue<int>("CustClosingBalanceUTCSevenParam:TimeZone"))
                            {
                                timeZone = _configuration.GetValue<int>("CustClosingBalanceUTCSevenParam:TimeZone");
                                startTime = _configuration.GetValue<string>("CustClosingBalanceUTCSevenParam:StartTime");
                                endTime = _configuration.GetValue<string>("CustClosingBalanceUTCSevenParam:EndTime");
                            }
                            break;
                        case 8:
                            //UTC+8
                            if (i == _configuration.GetValue<int>("CustClosingBalanceUTCEightParam:TimeZone"))
                            {
                                timeZone = _configuration.GetValue<int>("CustClosingBalanceUTCEightParam:TimeZone");
                                startTime = _configuration.GetValue<string>("CustClosingBalanceUTCEightParam:StartTime");
                                endTime = _configuration.GetValue<string>("CustClosingBalanceUTCEightParam:EndTime");
                            }
                            break;
                    }

                    #region Get BeginAt

                    beginAt = GetUTC8Time();
                    if (beginAt == null)
                    {
                        _log2File.Error($"CustClosingBalanceWorker-Check Run Or Not Error. TimeZone={timeZone} NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                        _logger.LogInformation($"Check Run Or Not Error At {startTime} - {endTime}，TimeZone={timeZone} NowTime(UTC+8)={runTime}");
                        
                        continue;
                    }

                    #endregion

                    today = Convert.ToDateTime(beginAt).Date.ToString();

                    if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {startTime}", out DateTime startDateTime) &&
                        DateTime.TryParse($"{beginAt:yyyy/MM/dd} {endTime}", out DateTime endDateTime))
                    {
                        _log2File.Info($"CustClosingBalanceWorker-Check Run Or Not :Get StartDateTime And EndDateTime Success. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                        _logger.LogInformation($"Check Run Or Not :Get StartDateTime And EndDateTime Success. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");

                        if (endDateTime < startDateTime)
                        {
                            endDateTime = endDateTime.AddDays(1);
                        }

                        switch (i)
                        {
                            case 7:
                                if (today != processUTCSevenDate) //Don't do it again.
                                {
                                    if(GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Run Or Not End :Not Run Yet. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Run Or Not End :Already Run. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Run Or Not End :Already Run At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                            case 8:
                                if (today != processUTCEightDate) //Don't do it again.
                                {
                                    if (GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Not Run Yet. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Already Run. StartDateTime={startDateTime}，EndDateTime={endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End : Already Run At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                        }
                    }
                }

                #endregion

                if (flag)
                {
                    _log2File.Info($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime}，TimeZone={timeZone}.", "CustClosingBalanceWorker");
                    _logger.LogInformation($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime}，TimeZone={timeZone}.");

                    int totalCount = _custClosingBalanceBLL.SetCustClosingBalance();

                    switch (timeZone)
                    {
                        case 7:
                            processUTCSevenDate = today;
                            coldDownTime = _configuration.GetValue<int>("CustClosingBalanceUTCSevenParam:ColdDownTime");
                            checkTime= _configuration.GetValue<int>("CustClosingBalanceUTCSevenParam:CheckTime");
                            break;
                        case 8:
                            processUTCEightDate = today;
                            coldDownTime = _configuration.GetValue<int>("CustClosingBalanceUTCEightParam:ColdDownTime");
                            checkTime = _configuration.GetValue<int>("CustClosingBalanceUTCEightParam:CheckTime");
                            break;
                    }

                    _log2File.Info($"CustClosingBalanceWorker-Run Success At Time(UTC+8)={runTime}, TimeZone={timeZone}, Affected Count:{totalCount}.", "CustClosingBalanceWorker");
                    _logger.LogInformation($"CustClosingBalanceWorker-Run Success At Time(UTC+8)={runTime}, TimeZone={timeZone}, Affected Count:{totalCount}.");

                }

                Thread.Sleep(coldDownTime * 1000);

                _log2File.Info($"CustClosingBalanceWorker-Run End At Time(UTC+8)={runTime}.TimeZone={timeZone}", "CustClosingBalanceWorker");
                _logger.LogInformation($"CustClosingBalanceWorker-Run End At Time(UTC+8)={runTime}.TimeZone={timeZone}");

            }
            catch (Exception ex)
            {
                _log2File.Error($"CustClosingBalanceWorker-Run Exception At Time(UTC+8)={runTime}.Message={ex.Message}", "CustClosingBalanceWorker");
                _logger.LogError($"CustClosingBalanceWorker-Run Exception At Time(UTC+8)={runTime}.Message={ex.Message}");
            }
            finally
            {
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(checkTime);
                _timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Taiwan Time(UTC+8)
        /// 台灣時間(UTC+8)
        /// </summary>
        /// <returns></returns>
        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }

        #endregion

    }
}
