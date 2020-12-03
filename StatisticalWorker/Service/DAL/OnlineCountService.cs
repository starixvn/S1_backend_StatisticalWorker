using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Workers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.DAL
{
    public class OnlineCountService
    {
        #region - property -
        private readonly ILogger<OnlineCountWorker> logger;
        private readonly IConfiguration configuration;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public OnlineCountService(IConfiguration configuration, Log2File log2File)
        {
            this.configuration = configuration;
            this.log2File = log2File;
        }
        #endregion

        #region - method -
        public IEnumerable<OnlineCountModel> GetOnlineOnlineUsersData()
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<OnlineCountModel>("Service_OnlineUsers_Get", commandType: CommandType.StoredProcedure);
        }

        public void SetOnlineCountData(IEnumerable<OnlineCountModel> para)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Execute("Service_OnlineCount_Set", para, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}
