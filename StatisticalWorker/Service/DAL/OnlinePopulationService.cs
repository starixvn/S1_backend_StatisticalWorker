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
    public class OnlinePopulationService
    {
        #region - property -
        private readonly ILogger<OnlinePopulationWorker> logger;
        private readonly IConfiguration configuration;
        private readonly Log2File log2File;
        #endregion

        #region - Construction method -
        public OnlinePopulationService(IConfiguration configuration, Log2File log2File)
        {
            this.configuration = configuration;
            this.log2File = log2File;
        }
        #endregion

        #region - method -
        public IEnumerable<OnlinePopulationModel> GetOnlinePopulationData(DateTime RecordTime)
        {
            var EndTime = RecordTime.AddMinutes(-10);
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<OnlinePopulationModel>("Service_OnlineAmung_Get", new { SYSStartDate = RecordTime, SYSEndDate = EndTime }, commandType: CommandType.StoredProcedure);
        }

        public void SetOnlinePopulationData(IEnumerable<OnlinePopulationModel> para)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Execute("Service_OnlinePopulation_Set", para, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}
