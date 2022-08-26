using Dapper;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model.enums;
using System.Data;

namespace StatisticalWorker.Service.DAL
{
    public class CustClosingBalanceService
    {
        private readonly MySqlClient _mainDbClient;

        public CustClosingBalanceService(IServiceProvider provider)
        {
            _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Main);
        }

        public int SetCustClosingBalanceData()
        {
            var totalCount = 0;
            var parameters = new DynamicParameters();
            parameters.Add("TotalCount", totalCount, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            string SpName = "Service_CustClosingBalance_Set";
            _mainDbClient.Execute(SpName, parameters);

            totalCount = parameters.Get<int?>("TotalCount") ?? 0;
            return totalCount;          
        }
    }
}
