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

        private string processNovaDate = string.Empty;
        private string processGoalDate = string.Empty;
        private string processBwingDate = string.Empty;
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

            int? siteId = null;
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

                for (int i = 1; i <= 3 && flag==false ; i++)
                {
                    switch (i)
                    {
                        case 1:
                            //Nova88
                            if (i == _configuration.GetValue<int>("CustClosingBalanceNovaParam:SiteId"))
                            {
                                siteId = _configuration.GetValue<int>("CustClosingBalanceNovaParam:SiteId");
                                timeZone = _configuration.GetValue<int>("CustClosingBalanceNovaParam:TimeZone");
                                startTime = _configuration.GetValue<string>("CustClosingBalanceNovaParam:StartTime");
                                endTime = _configuration.GetValue<string>("CustClosingBalanceNovaParam:EndTime");
                            }
                            break;
                        case 2:
                            //Goal123
                            if (i == _configuration.GetValue<int>("CustClosingBalanceGoalParam:SiteId"))
                            {
                                siteId = _configuration.GetValue<int>("CustClosingBalanceGoalParam:SiteId");
                                timeZone = _configuration.GetValue<int>("CustClosingBalanceGoalParam:TimeZone");
                                startTime = _configuration.GetValue<string>("CustClosingBalanceGoalParam:StartTime");
                                endTime = _configuration.GetValue<string>("CustClosingBalanceGoalParam:EndTime");
                            }
                            break;
                        case 3:
                            //Bwing
                            if (i == _configuration.GetValue<int>("CustClosingBalanceBwingParam:SiteId"))
                            {
                                siteId = _configuration.GetValue<int>("CustClosingBalanceBwingParam:SiteId");
                                timeZone = _configuration.GetValue<int>("CustClosingBalanceBwingParam:TimeZone");
                                startTime = _configuration.GetValue<string>("CustClosingBalanceBwingParam:StartTime");
                                endTime = _configuration.GetValue<string>("CustClosingBalanceBwingParam:EndTime");
                            }
                            break;
                    }

                    #region Get BeginAt

                    beginAt = timeZone != null ? GetUTCTimeByTimeZone((int)timeZone) : null;
                    if (beginAt == null)
                    {
                        string siteName = GetSiteName(siteId);

                        _log2File.Error($"CustClosingBalanceWorker-Check Site Run Or Not Error.SiteId={siteId}，SiteName={siteName}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                        _logger.LogInformation($"Check Site Run Or Not Error At {startTime} - {endTime}，NowTime(UTC+8)={runTime}");
                        
                        continue;
                    }

                    #endregion

                    today = Convert.ToDateTime(beginAt).Date.ToString();

                    if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {startTime}", out DateTime startDateTime) &&
                        DateTime.TryParse($"{beginAt:yyyy/MM/dd} {endTime}", out DateTime endDateTime))
                    {
                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not :Get StartDateTime And EndDateTime Success. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                        _logger.LogInformation($"Check Site Run Or Not :Get StartDateTime And EndDateTime Success. StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}");

                        if (endDateTime < startDateTime)
                        {
                            endDateTime = endDateTime.AddDays(1);
                        }

                        switch (i)
                        {
                            case 1:
                                if (today != processNovaDate) //Don't do it again.
                                {
                                    if(GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Not Run Yet. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Already Run. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Already Run At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                            case 2:
                                if (today != processGoalDate) //Don't do it again.
                                {
                                    if (GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Not Run Yet. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Already Run. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End : Already Run At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                            case 3:
                                if (today != processBwingDate) //Don't do it again.
                                {
                                    if (GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Not Run Yet. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End: Not Run Yet At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _log2File.Info($"CustClosingBalanceWorker-Check Site Run Or Not End :Already Run. SiteId={siteId}，SiteName={GetSiteName(siteId)}，StartDateTime={startDateTime}，EndDateTime={endDateTime}，NowTime(UTC+8)={runTime}", "CustClosingBalanceWorker");
                                        _logger.LogInformation($"Check Site Run Or Not End :Already Run At {startDateTime} - {endDateTime}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                        }
                    }
                }

                #endregion

                if (flag)
                {
                    _log2File.Info($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime} With SiteId={siteId}, SiteName={GetSiteName(siteId)}, TimeZone={timeZone}.", "CustClosingBalanceWorker");
                    _logger.LogInformation($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime} With SiteId={siteId}, SiteName={GetSiteName(siteId)}, TimeZone={timeZone}.");

                    int totalCount = _custClosingBalanceBLL.SetCustClosingBalance(
                                     new CustClosingBalanceParameterModel()
                                     {
                                         SiteId = (int)siteId,
                                         TimeZone = timeZone>=0 ? $"+{timeZone.ToString()}:00" : $"{timeZone.ToString()}:00"
                                     });

                    switch (siteId)
                    {
                        case 1:
                            processNovaDate = today;
                            coldDownTime = _configuration.GetValue<int>("CustClosingBalanceNovaParam:ColdDownTime");
                            checkTime= _configuration.GetValue<int>("CustClosingBalanceNovaParam:CheckTime");
                            break;
                        case 2:
                            processGoalDate = today;
                            coldDownTime = _configuration.GetValue<int>("CustClosingBalanceGoalParam:ColdDownTime");
                            checkTime = _configuration.GetValue<int>("CustClosingBalanceGoalParam:CheckTime");
                            break;
                        case 3:
                            processBwingDate = today;
                            coldDownTime = _configuration.GetValue<int>("CustClosingBalanceBwingParam:ColdDownTime");
                            checkTime = _configuration.GetValue<int>("CustClosingBalanceBwingParam:CheckTime");
                            break;
                    }

                    _log2File.Info($"CustClosingBalanceWorker-Run Success At Time(UTC+8)={runTime} With SiteId={siteId}, SiteName={GetSiteName(siteId)}, TimeZone={timeZone}, Affected Count:{totalCount}.", "CustClosingBalanceWorker");
                    _logger.LogInformation($"CustClosingBalanceWorker-Run Success At Time(UTC+8)={runTime} With SiteId={siteId}, SiteName={GetSiteName(siteId)}, TimeZone={timeZone}, Affected Count:{totalCount}.");

                }

                Thread.Sleep(coldDownTime * 1000);

                _log2File.Info($"CustClosingBalanceWorker-Run End At Time(UTC+8)={runTime}.", "CustClosingBalanceWorker");
                _logger.LogInformation($"CustClosingBalanceWorker-Run End At Time(UTC+8)={runTime}.");

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

        /// <summary>
        /// Get UTC Time With TimeZone
        /// 取得時區時間
        /// </summary>
        /// <param name="timeZone">時區</param>
        /// <returns></returns>
        private DateTime GetUTCTimeByTimeZone(int timeZone)
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(timeZone, 0, 0)).DateTime;
        }

        /// <summary>
        /// Get SiteName
        /// 取得伺服器名稱
        /// </summary>
        /// <param name="siteId">伺服器ID</param>
        /// <returns></returns>
        private string GetSiteName(int? siteId)
        {
            switch (siteId)
            {
                case 1:
                    return "Nova88";
                case 2:
                    return "Goal123";
                case 3:
                    return "Bwing";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Parameter Check Step
        /// </summary>
        /// <param name="tablecheck">Table Name</param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="table"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private bool CheckParameter(bool tablecheck,
                                    string sDate,
                                    string eDate,
                                    out bool table,
                                    out string startDate,
                                    out string endDate)
        {
            startDate = string.Empty;
            endDate = string.Empty;
            table = tablecheck;

            if (tablecheck)
            {
                //TpBetRecord14

                if (String.IsNullOrEmpty(sDate) || String.IsNullOrEmpty(eDate))
                {
                    return false;
                }
                else
                {
                    startDate = sDate;
                    endDate = eDate;
                }
            }
            else
            {
                //TpBetRecord

                if (String.IsNullOrEmpty(sDate))
                {
                    startDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                }
                else
                {
                    startDate = sDate;
                }

                if (String.IsNullOrEmpty(eDate))
                {
                    endDate = DateTime.Today.AddDays(-14).ToString("yyyy-MM-dd");
                }
                else
                {
                    endDate = eDate;
                }
            }

            return true;
        }

        #endregion

    }
}
