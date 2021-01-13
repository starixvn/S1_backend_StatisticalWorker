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
        public void SetLeaderBoardTableV2(IEnumerable<LeaderBoardModelV2> leaderBoardData)
        {
            setLeaderBoardTableService.SetLeaderBoardTableV2(leaderBoardData);
        }
        public IEnumerable<LeaderBoardModelV2> GetLeaderBoardDataV2(int is14, int siteId, string eventCode, string startDate, string endDate, string timeZone, string sPName)
        {
            return setLeaderBoardTableService.GetLeaderBoardDataV2(is14, siteId, eventCode, startDate, endDate, timeZone, sPName);
        }
        public IEnumerable<LeaderBoardEvent> GetLeaderboardEvent()
        {
            return setLeaderBoardTableService.GetLeaderboardEvent();
        }
        public void CheckLeaderboardOverTime()
        {
            setLeaderBoardTableService.CheckLeaderboardOverTime();
        }
        #endregion
    }
}