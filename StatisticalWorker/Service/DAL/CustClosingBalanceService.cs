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
    public class CustClosingBalanceService
    {
        #region property

        private readonly IConfiguration _configuration;

        #endregion

        #region Construction method

        public CustClosingBalanceService(IConfiguration configuration)
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
        public int SetCustClosingBalanceData(CustClosingBalanceParameterModel param)
        {
            int totalCount = 0;

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                var parameters = new DynamicParameters(
                new
                {
                    _SiteId = param.SiteId,
                    _TimeZone = param.TimeZone
                });
                parameters.Add("TotalCount", totalCount, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                string SpName = "Service_CustClosingBalance_Set";
                conn.Execute(SpName, parameters, commandType: CommandType.StoredProcedure);

                totalCount = parameters.Get<int?>("TotalCount") ?? 0;

                return totalCount;
            }
        }

        #endregion
    }
}
