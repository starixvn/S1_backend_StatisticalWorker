using StatisticalWorker.Service.Library;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Service.Model.enums;

namespace StatisticalWorker.Service.DAL
{
    public class OnlineCountService
    {
        private readonly MySqlClient _mainDbClient;
        private readonly MySqlClient _reportDbClient;
        public OnlineCountService(IServiceProvider provider)
        {
            _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Main);
            _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Report);
        }

        public IEnumerable<OnlineCountModel> GetOnlineOnlineUsersData()
        {         
            return _reportDbClient.Query<OnlineCountModel>("Service_OnlineUsers_Get_V2");
        }

        public void SetOnlineCountData(IEnumerable<OnlineCountModel> para)
        {
            _mainDbClient.Execute("Service_OnlineCount_Set_V2", para);
        }
    }
}
