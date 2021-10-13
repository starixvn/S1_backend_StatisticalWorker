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
    public class TpBetRecordRelayForReportWorker : BackgroundService
    {
        #region property

        private Timer _timer;

        private readonly ILogger<TpBetRecordRelayForReportWorker> _logger;
        private readonly Log2File _log2File;
        private readonly TpBetRecordRelayBLL _tpBetRecordRelayBLL;
        private readonly IConfiguration _configuration;
        private string processDate = string.Empty;

        private int execCount = 0;

        #endregion

        #region Construction method

        public TpBetRecordRelayForReportWorker(ILogger<TpBetRecordRelayForReportWorker> logger,
                                               Log2File log2File,
                                               TpBetRecordRelayBLL tpBetRecordRelayBLL,
                                               IConfiguration configuration)
        {
            _logger = logger;
            _log2File = log2File;
            _tpBetRecordRelayBLL = tpBetRecordRelayBLL;
            _configuration = configuration;
        }

        #endregion

        #region protected method 

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(RelayTpBetRecordForReport, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }

        #endregion

        #region private method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void RelayTpBetRecordForReport(object state)
        {
            Interlocked.Increment(ref execCount);

            if (execCount == 1)
            {
                var beginAt = GetUTC8Time();
                string today = beginAt.Date.ToString();

                _log2File.Info($"TpBetRecordRelayForReportWorker- Thread Start. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.", "TpBetRecordRelayForReportWorker");
                _logger.LogInformation($"TpBetRecordRelayForReportWorker- Thread Start. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.");

                try
                {
                    if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {_configuration.GetValue<string>("TpBetRecordRelayParam:StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {_configuration.GetValue<string>("TpBetRecordRelayParam:EndTime")}", out DateTime endTime))
                    {
                        _log2File.Info($"TpBetRecordRelayForReportWorker- Parameter Get Success. StartTime={startTime}，EndTime={endTime}，BeginAt={beginAt}.", "TpBetRecordRelayForReportWorker");
                        _logger.LogInformation($"TpBetRecordRelayForReportWorker- Parameter Get Success. StartTime={startTime}，EndTime={endTime}，BeginAt={beginAt}.");

                        if (endTime < startTime)
                        {
                            endTime = endTime.AddDays(1);
                        }

                        if (today != processDate) //Don't do it again.
                        {

                            _log2File.Info($"TpBetRecordRelayForReportWorker- Run Start. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.", "TpBetRecordRelayForReportWorker");
                            _logger.LogInformation($"TpBetRecordRelayForReportWorker- Run Start. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.");

                            if (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                            {
                                //#region CheckParameter

                                //if (!CheckParameter(false,
                                //               _configuration.GetValue<string>("TpBetRecordRelayParam:StartDate"),
                                //               _configuration.GetValue<string>("TpBetRecordRelayParam:EndDate"),
                                //               out bool table,
                                //               out string startDate,
                                //               out string endDate))
                                //{
                                //    _log2File.Info(string.Format("TpBetRecordRelayForReportWorker-{0} Parameter Check Fail. ", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")), "TpBetRecordRelayForReportWorker");
                                //    _logger.LogInformation(string.Format("TpBetRecordRelayForReportWorker-{0}  Parameter Check Fail.", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")));

                                //    return;
                                //}

                                //#endregion

                                _log2File.Info($"TpBetRecordRelayForReportWorker- Judge Success. result is need to run. {GetUTC8Time()} >= {startTime} && {GetUTC8Time()} <= {endTime}. {GetUTC8Time().ToString("yyyy / MM / dd HH: mm:ss")}.", "TpBetRecordRelayForReportWorker");
                                _logger.LogInformation($"TpBetRecordRelayForReportWorker- Judge Success. result is need to run. {GetUTC8Time()} >= {startTime} && {GetUTC8Time()} <= {endTime}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.");


                                int timeoutSeconds = _configuration.GetValue<int?>("TpBetRecordRelayParam:TimeOutSeconds") == null
                                                     ? 30
                                                     : (int)_configuration.GetValue<int?>("TpBetRecordRelayParam:TimeOutSeconds");

                                _log2File.Info($"TpBetRecordRelayForReportWorker- TimeOutSeconds = {timeoutSeconds}", "TpBetRecordRelayForReportWorker");
                                _logger.LogInformation($"TpBetRecordRelayForReportWorker- TimeOutSeconds = {timeoutSeconds}.");

                                //temp log: 先補紀錄用(ELK上可以查詢，不用上Server調log)
                                _log2File.Error($"TpBetRecordRelayForReportWorker- TimeOutSeconds = {timeoutSeconds}", "TpBetRecordRelayForReportWorker");
                                _logger.LogError($"TpBetRecordRelayForReportWorker- TimeOutSeconds = {timeoutSeconds}.");


                                int totalCount = _tpBetRecordRelayBLL.SetTpBetRecordRelayForReport(
                                                 new TpBetRecordRelayForReportParameterModel()
                                                 {
                                                     Table = false,
                                                     StartDate = string.Empty,
                                                     EndDate = string.Empty,
                                                     TimeoutSeconds = timeoutSeconds
                                                 });

                                _log2File.Info($"TpBetRecordRelayForReportWorker- Run Success. Affected Count:{totalCount}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}", "TpBetRecordRelayForReportWorker");
                                _logger.LogInformation($"TpBetRecordRelayForReportWorker- Run Success.Affected Count:{totalCount}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");

                                //temp log: 先補紀錄用(ELK上可以查詢，不用上Server調log)
                                _log2File.Error($"TpBetRecordRelayForReportWorker- Run Success. Affected Count:{totalCount}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}", "TpBetRecordRelayForReportWorker");
                                _logger.LogError($"TpBetRecordRelayForReportWorker- Run Success.Affected Count:{totalCount}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");

                                processDate = today;

                                Thread.Sleep(_configuration.GetValue<int>("TpBetRecordRelayParam:ColdDownTime") * 1000);
                            }
                            else
                            {
                                _log2File.Info($"TpBetRecordRelayForReportWorker- Judge Success. result is can't run. {GetUTC8Time()} >= {startTime} && {GetUTC8Time()} <= {endTime}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}", "TpBetRecordRelayForReportWorker");
                                _logger.LogInformation($"TpBetRecordRelayForReportWorker- Judge Success. result is can't run. {GetUTC8Time()} >= {startTime} && {GetUTC8Time()} <= {endTime}. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log2File.Error($"TpBetRecordRelayForReportWorker- Error：{ex.Message}. {ex}", "TpBetRecordRelayForReportWorker");
                    _logger.LogError(ex, ex.Message);

                }
                finally
                {
                    _log2File.Info($"TpBetRecordRelayForReportWorker- Run End. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}", "TpBetRecordRelayForReportWorker");
                    _logger.LogInformation($"TpBetRecordRelayForReportWorker- Run End. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}");
                    TimeSpan dueTimeSpan = TimeSpan.FromSeconds(_configuration.GetValue<int>("TpBetRecordRelayParam:CheckTime"));
                    _timer.Change(dueTimeSpan, TimeSpan.Zero);
                }
                _log2File.Info($"TpBetRecordRelayForReportWorker- Thread End. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.", "TpBetRecordRelayForReportWorker");
                _logger.LogInformation($"TpBetRecordRelayForReportWorker- Thread End. {GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}.");
            }
            Interlocked.Decrement(ref execCount);
        }

        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
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
