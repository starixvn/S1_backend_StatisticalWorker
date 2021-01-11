using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.DAL
{
    public class TpBetRecordRelayService
    {
        #region property

        private readonly IConfiguration _configuration;

        #endregion

        #region Construction method

        public TpBetRecordRelayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region method

        /// <summary>
        /// Select data And Insert its
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Affected row counts</returns>
        public int SetTpBetRecordRelayData(TpBetRecordRelayForReportParameterModel param)
        {
            int totalCount = 0;

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("ConnectionStringReport")))
            {
                var parameters = new DynamicParameters(
                new
                {
                    _Table = param.Table,
                    _StartDate = param.StartDate,
                    _EndDate = param.EndDate
                });
                parameters.Add("TotalCount", totalCount, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                string SpName = "Relay_Report_TpBetRecord_Set";
                conn.Execute(SpName, parameters, commandType: CommandType.StoredProcedure);

                totalCount = parameters.Get<int?>("TotalCount") ?? 0;
                return totalCount;
            }
        }

        #endregion
    }
}
