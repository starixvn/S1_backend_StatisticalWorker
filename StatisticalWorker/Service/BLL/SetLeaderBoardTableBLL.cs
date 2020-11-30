using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.BLL
{
    public class SetLeaderBoardTableBLL
    {
        #region - property -
        private readonly SetLeaderBoardTableService setLeaderBoardTableService;
        #endregion

        #region - Construction method -
        public SetLeaderBoardTableBLL(SetLeaderBoardTableService setLeaderBoardTableService)
        {
            this.setLeaderBoardTableService = setLeaderBoardTableService;
        }
        #endregion

        #region - method -        
        public void SetLeaderBoardTable(IEnumerable<LeaderBoardModel> LeaderBoardData)
        {
            setLeaderBoardTableService.SetLeaderBoardTableDll(LeaderBoardData);
        }

        public IEnumerable<LeaderBoardModel> GetLeaderBoardData()
        {
            return setLeaderBoardTableService.GetLeaderBoardDataDll();
        }
        #endregion
    }
}