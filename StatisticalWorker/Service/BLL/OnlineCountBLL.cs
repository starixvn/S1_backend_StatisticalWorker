using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;

namespace StatisticalWorker.Service.BLL
{
    public class OnlineCountBLL
    {
        private readonly OnlineCountService _onlinePopulationService;

        public OnlineCountBLL(IServiceProvider provider)
        {
            _onlinePopulationService = provider.GetRequiredService<OnlineCountService>();
        }

        public IEnumerable<OnlineCountModel> GetOnlineOnlineUsersData()
        {
            return _onlinePopulationService.GetOnlineOnlineUsersData();
        }

        public void SetOnlineCountData(IEnumerable<OnlineCountModel> para)
        {
            _onlinePopulationService.SetOnlineCountData(para);
        }

    }
}
