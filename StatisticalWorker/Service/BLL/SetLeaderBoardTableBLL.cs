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

        public IEnumerable<LeaderBoardModel> GetN88LeaderBoardData(int is14)
        {
            return setLeaderBoardTableService.GetN88LeaderBoardDataDll(is14);
        }

        public IEnumerable<LeaderBoardModel> GetBGLeaderBoardData(int is14)
        {
            return setLeaderBoardTableService.GetBGLeaderBoardDataDll(is14);
        }

        public void SetLeaderBoardDeposit(IEnumerable<DepositModel> DepositData)
        {
            setLeaderBoardTableService.SetLeaderBoardDepositDll(DepositData);
        }

        public IEnumerable<DepositModel> GetN88LeaderBoardDeposit()
        {
            return setLeaderBoardTableService.GetN88LeaderBoardDepositDll();
        }

        public IEnumerable<DepositModel> GetBGLeaderBoardDeposit()
        {
            return setLeaderBoardTableService.GetBGLeaderBoardDepositDll();
        }
        #endregion
    }
}