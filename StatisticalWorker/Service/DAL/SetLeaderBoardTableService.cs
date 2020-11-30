using Dapper;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.DAL
{
    public class SetLeaderBoardTableService
    {
        #region - property -
        private readonly ILogger<LeaderBoardWorker> logger;
        private readonly IConfiguration configuration;
        #endregion

        #region - Construction method -
        public SetLeaderBoardTableService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #endregion

        #region - method -
        public void SetLeaderBoardTableDll(IEnumerable<LeaderBoardModel> LeaderBoardData)
        {           
            //至多重新執行 N 次
            int RtyTimes = 2;
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStrings"));
            for(int i = 0; i < RtyTimes; ++i)
            {
                try
                {
                    foreach (var leaderBoardData in LeaderBoardData)
                    {
                        conn.Execute("Service_Leaderboard_Set", leaderBoardData, commandType: CommandType.StoredProcedure);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (i == RtyTimes - 1)
                    {
                        logger.LogError(ex.Message);
                        break;
                    }
                }
            }                      
        }
        public IEnumerable<LeaderBoardModel> GetLeaderBoardDataDll()
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<LeaderBoardModel>("Service_Leaderboard_Get", null, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}
