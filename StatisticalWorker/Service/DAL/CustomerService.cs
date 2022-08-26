using Dapper;
using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Service.Model.enums;

namespace StatisticalWorker.Service.DAL
{
    public class CustomerService
    {
        private readonly MySqlClient _mainDbClient;
        private readonly MySqlClient _reportDbClient;
        public CustomerService(IServiceProvider provider)
        {
            _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Main);
            _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Report);
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