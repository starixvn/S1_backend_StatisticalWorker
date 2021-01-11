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

            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();
            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {_configuration.GetValue<string>("TpBetRecordRelayParam:StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {_configuration.GetValue<string>("TpBetRecordRelayParam:EndTime")}", out DateTime endTime))
                {
                    _log2File.Info($"TpBetRecordRelayForReportWorker-StartTime={startTime}，EndTime={endTime}，BeginAt={GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}", "TpBetRecordRelayForReportWorker");
                    _logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processDate) //Don't do it again.
                    {

                        _log2File.Info($"TpBetRecordRelayForReportWorker-{GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}  Run Start.", "TpBetRecordRelayForReportWorker");
                        _logger.LogInformation($"TpBetRecordRelayForReportWorker-{GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")}  Run Start.");

                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            #region CheckParameter

                            if (!CheckParameter(false,
                                           _configuration.GetValue<string>("TpBetRecordRelayParam:StartDate"),
                                           _configuration.GetValue<string>("TpBetRecordRelayParam:EndDate"),
                                           out bool table,
                                           out string startDate,
                                           out string endDate))
                            {
                                _log2File.Info(string.Format("TpBetRecordRelayForReportWorker-{0} Parameter Check Fail. ", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")), "TpBetRecordRelayForReportWorker");
                                _logger.LogInformation(string.Format("TpBetRecordRelayForReportWorker-{0}  Parameter Check Fail.", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss")));

                                return;
                            }

                            #endregion

                            int totalCount = _tpBetRecordRelayBLL.SetTpBetRecordRelayForReport(
                                             new TpBetRecordRelayForReportParameterModel()
                                             {
                                                 Table = table,
                                                 StartDate = startDate,
                                                 EndDate = endDate
                                             });

                            _log2File.Info(string.Format("TpBetRecordRelayForReportWorker-{0} Run Success. Affected Count:{1}", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss"), totalCount), "TpBetRecordRelayForReportWorker");
                            _logger.LogInformation(string.Format("TpBetRecordRelayForReportWorker-{0}  Run Success.Affected Count:{1}", GetUTC8Time().ToString("yyyy/MM/dd HH:mm:ss"), totalCount));

                            processDate = today;

                            Thread.Sleep(_configuration.GetValue<int>("DGArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);

            }
            finally
            {
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(_configuration.GetValue<int>("TpBetRecordRelayParam:CheckTime"));
                _timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
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
                    startDate = DateTime.Today.ToString("yyyy-MM-dd");
                }
                else
                {
                    startDate = sDate;
                }

                if (String.IsNullOrEmpty(eDate))
                {
                    endDate = DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd");
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
