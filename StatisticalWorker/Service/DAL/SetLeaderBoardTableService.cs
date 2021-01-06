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
using StatisticalWorker.Service.Library;

namespace StatisticalWorker.Service.DAL
{
    public class SetLeaderBoardTableService
    {
        #region - property -
        private readonly ILogger<LeaderBoardWorker> logger;
        private readonly IConfiguration configuration;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public SetLeaderBoardTableService(IConfiguration configuration, ILogger<LeaderBoardWorker> logger, Log2File log2File)
        {
            this.configuration = configuration;
            this.log2File = log2File;
            this.logger = logger;
        }
        #endregion

        #region - method -
        public void SetLeaderBoardTableDll(IEnumerable<LeaderBoardModel> LeaderBoardData)
        {
            //至多重新執行 N 次
            int RtyTimes = 2;
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            for (int i = 0; i < RtyTimes; ++i)
            {
                try
                {
                    conn.Execute("Service_Leaderboard_Set", LeaderBoardData, commandType: CommandType.StoredProcedure);
                    break;
                }
                catch (Exception ex)
                {
                    if (i == RtyTimes - 1)
                    {
                        log2File.Error($"{i + 1} Time\r\n{ex}", "LeaderBoardWorkerLog");
                        logger.LogError(ex.Message);
                        break;
                    }
                }
            }
        }
        public IEnumerable<LeaderBoardModel> GetN88LeaderBoardDataDll(int is14)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<LeaderBoardModel>("Service_Leaderboard_N88Get", new { Is14 = is14 }, commandTimeout: 180, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<LeaderBoardModel> GetBGLeaderBoardDataDll(int is14)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<LeaderBoardModel>("Service_Leaderboard_BGGet", new { Is14 = is14 }, commandTimeout: 180, commandType: CommandType.StoredProcedure);
        }

        public void SetLeaderBoardDepositDll(IEnumerable<DepositModel> DepositData)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Execute("Service_LeaderboardDeposit_Set", DepositData, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<DepositModel> GetN88LeaderBoardDepositDll()
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<DepositModel>("Service_LeaderboardDeposit_N88Get", null, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<DepositModel> GetBGLeaderBoardDepositDll()
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<DepositModel>("Service_LeaderboardDeposit_BGGet", null, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}
