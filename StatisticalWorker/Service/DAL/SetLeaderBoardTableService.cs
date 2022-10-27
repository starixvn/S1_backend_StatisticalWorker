using Dapper;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Workers;
using MySql.Data.MySqlClient;
using System.Data;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model.enums;

namespace StatisticalWorker.Service.DAL
{
    public class SetLeaderBoardTableService
    {
        private readonly ILogger<LeaderBoardWorker> _logger;
        private readonly MySqlClient _mainDbClient;
        private readonly MySqlClient _reportDbClient;

        public SetLeaderBoardTableService(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<LeaderBoardWorker>>();
            _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Main);
            _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Report);
        }


        public void SetLeaderBoardTableDll(IEnumerable<LeaderBoardModel> LeaderBoardData)
        {
            //至多重新執行 N 次
            int RtyTimes = 2;

            for (int i = 0; i < RtyTimes; ++i)
            {
                try
                {
                    _mainDbClient.Execute("Service_Leaderboard_Set", LeaderBoardData);
                    break;
                }
                catch (Exception ex)
                {
                    if (i == RtyTimes - 1)
                    {
                        _logger.LogError(ex, ex.Message);
                        break;
                    }
                }
            }
        }

        public void SetLeaderBoardTableV2(IEnumerable<LeaderBoardModelV2> LeaderBoardData)
        {
            //至多重新執行 N 次
            int RtyTimes = 2;
            for (int i = 0; i < RtyTimes; ++i)
            {
                try
                {
                    _mainDbClient.Execute("Service_LeaderboardV2_Set", LeaderBoardData);
                    break;
                }
                catch (Exception ex)
                {
                    if (i == RtyTimes - 1)
                    {
                        _logger.LogError(ex, ex.Message);
                        break;
                    }
                }
            }
        }

        public IEnumerable<LeaderBoardModelV2> GetLeaderBoardDataV2(int is14, int siteId, string eventCode, string startDate, string endDate, string timeZone, string sPName)
        {
            return _reportDbClient.Query<LeaderBoardModelV2>(sPName, new { SiteId = siteId, EventCode = eventCode, StartDate = startDate, EndDate = endDate, TimeZone = timeZone, Is14 = is14 }, commandTimeout: 300);
        }

        public IEnumerable<LeaderBoardModelV2> GetLeaderBoardExtraPoint(int siteId, string eventCode, string startDate, string endDate, string timeZone, string sPName)
        {

            if (sPName != null && sPName != "")
            {
                return _reportDbClient.Query<LeaderBoardModelV2>(sPName, new { SiteId = siteId, EventCode = eventCode, StartDate = startDate, EndDate = endDate, TimeZone = timeZone });
            }
            else
            {
                return _reportDbClient.Query<LeaderBoardModelV2>("Service_LeaderboardExtraPoint_Get", new { SiteId = siteId, EventCode = eventCode, StartDate = startDate, EndDate = endDate, TimeZone = timeZone });
            }
        }

        public IEnumerable<LeaderBoardEvent> GetLeaderboardEvent()
        {
            return _reportDbClient.Query<LeaderBoardEvent>("Service_LeaderboardEvent_Get");
        }

        public void CheckLeaderboardOverTime()
        {
            _mainDbClient.Execute("Service_LeaderboardOverTime_Set");
        }

    }
}
