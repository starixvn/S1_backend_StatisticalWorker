using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.BLL
{
    public class OnlineCountBLL
    {
        #region - property -
        private readonly OnlineCountService onlinePopulationService;
        #endregion

        #region - Construction method -
        public OnlineCountBLL(OnlineCountService onlinePopulationService)
        {
            this.onlinePopulationService = onlinePopulationService;
        }
        #endregion

        #region - method -
        public IEnumerable<OnlineCountModel> GetOnlineOnlineUsersData()
        {
            return onlinePopulationService.GetOnlineOnlineUsersData();
        }

        public void SetOnlineCountData(IEnumerable<OnlineCountModel> para)
        {
            onlinePopulationService.SetOnlineCountData(para);
        }
        #endregion
    }
}
