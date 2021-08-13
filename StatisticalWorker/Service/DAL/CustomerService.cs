using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;

namespace StatisticalWorker.Service.DAL
{
    public class CustomerService
    {
        private readonly MySqlClient _mainDbClient;
        private readonly MySqlClient _reportDbClient;
        public CustomerService(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var mySqlLogger = serviceProvider.GetRequiredService<ILogger<MySqlClient>>();
            _mainDbClient = new MySqlClient(configuration.GetConnectionString("ConnectionString"), mySqlLogger);
            _reportDbClient = new MySqlClient(configuration.GetConnectionString("ConnectionStringReport"), mySqlLogger);
        }
        
        public IEnumerable<DormantSettingOutput> GetDormantSetting()
        {
            return _reportDbClient.Query<DormantSettingOutput>("Service_DormantSetting_Get");
        }
        
        public IEnumerable<DormantAccountOutput> GetDormantAccount(DormantAccountInput dormantAccountInput)
        {
            var parameters = new DynamicParameters(new {dormantAccountInput.SiteId, dormantAccountInput.Day, dormantAccountInput.StatusList});
            return _reportDbClient.Query<DormantAccountOutput>("Service_DormantAccount_Get", parameters, 180);
        }
        
        public int DormantSettingSet(DormantSettingInput dormantSettingInput)
        {
            var parameters = new DynamicParameters(new {dormantSettingInput.SiteId});
            return _mainDbClient.Execute("Service_DormantSetting_Set", parameters);
        }

        public int LockDormantAccountSet(LockDormantAccountInput lockDormantAccountInput)
        {
            var parameters = new DynamicParameters(new {lockDormantAccountInput.CustId, lockDormantAccountInput.Status});
            return _mainDbClient.Execute("Service_LockDormantAccount_Set", parameters);
        }
        
        public int CustNotesSet(CustNotesInput custNotesInput)
        {
            var parameters = new DynamicParameters(new {custNotesInput.CustId, custNotesInput.Note, custNotesInput.Operator});
            return _mainDbClient.Execute("Admin_CustNotes_Ins", parameters);
        }
    }
}