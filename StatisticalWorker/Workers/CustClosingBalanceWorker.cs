using StatisticalWorker.Service.BLL;

namespace StatisticalWorker.Workers
{
    public class CustClosingBalanceWorker : BackgroundService
    {
        private Timer _timer;
        private readonly ILogger<CustClosingBalanceWorker> _logger;
        private readonly CustClosingBalanceBLL _custClosingBalanceBLL;
        private readonly IConfiguration _configuration;
        private string processUTCSevenDate = string.Empty;
        private string processUTCEightDate = string.Empty;

        public CustClosingBalanceWorker(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<CustClosingBalanceWorker>>();
            _custClosingBalanceBLL = provider.GetRequiredService<CustClosingBalanceBLL>();
            _configuration = provider.GetRequiredService<IConfiguration>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(SetCustClosingBalance,
                         null,
                               TimeSpan.Zero,
                               TimeSpan.Zero);

            return Task.CompletedTask;
        }

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
                _logger.LogInformation($"CustClosingBalanceWorker-Run Start At Time(UTC+8)={runTime}.");

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

                    beginAt = GetUTC8Time();
                    if (beginAt == null)
                    {                        
                        _logger.LogInformation($"Check Run Or Not Error At {startTime} - {endTime}，TimeZone={timeZone} NowTime(UTC+8)={runTime}");                        
                        continue;
                    }

                    today = Convert.ToDateTime(beginAt).Date.ToString();

                    if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {startTime}", out DateTime startDateTime) &&
                        DateTime.TryParse($"{beginAt:yyyy/MM/dd} {endTime}", out DateTime endDateTime))
                    {
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
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");

                                        flag = true;
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"Check Run Or Not End :Already Run At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                            case 8:
                                if (today != processUTCEightDate) //Don't do it again.
                                {
                                    if (GetUTC8Time() >= startDateTime && GetUTC8Time() <= endDateTime)
                                    {
                                        _logger.LogInformation($"Check Site Run Or Not End :Not Run Yet At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");
                                        flag = true;
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"Check Site Run Or Not End : Already Run At {startDateTime} - {endDateTime}，TimeZone={timeZone}，NowTime(UTC+8)={runTime}");
                                    }
                                }
                                break;
                        }
                    }
                }

                if (flag)
                {
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

                    _logger.LogInformation($"CustClosingBalanceWorker-Run Success At Time(UTC+8)={runTime}, TimeZone={timeZone}, Affected Count:{totalCount}.");

                }

                Thread.Sleep(coldDownTime * 1000);
                _logger.LogInformation($"CustClosingBalanceWorker-Run End At Time(UTC+8)={runTime}.TimeZone={timeZone}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CustClosingBalanceWorker-Run Exception At Time(UTC+8)={runTime}.Message={ex.Message}");
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
            return new DateTimeOffset(DateTime.UtcNow).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }

    }
}
