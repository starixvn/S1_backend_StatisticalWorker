using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.BLL
{
    public class OnlinePopulationBLL
    {
        #region - property -
        private readonly OnlinePopulationService onlinePopulationService;
        #endregion

        #region - Construction method -
        public OnlinePopulationBLL(OnlinePopulationService onlinePopulationService)
        {
            this.onlinePopulationService = onlinePopulationService;
        }
        #endregion

        #region - method -
        public IEnumerable<OnlinePopulationModel> GetOnlinePopulationData(DateTime RecordTime)
        {
            return onlinePopulationService.GetOnlinePopulationData(RecordTime);
        }

        public void SetOnlinePopulationData(IEnumerable<OnlinePopulationModel> para)
        {
            onlinePopulationService.SetOnlinePopulationData(para);
        }
        #endregion
    }
}
