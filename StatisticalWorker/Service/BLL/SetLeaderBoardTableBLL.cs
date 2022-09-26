using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;

namespace StatisticalWorker.Service.BLL
{
    public class SetLeaderBoardTableBLL
    {
        private readonly SetLeaderBoardTableService _setLeaderBoardTableService;

        public SetLeaderBoardTableBLL(IServiceProvider provider)
        {
            _setLeaderBoardTableService = provider.GetRequiredService<SetLeaderBoardTableService>(); 
        }

        public void SetLeaderBoardTableV2(IEnumerable<LeaderBoardModelV2> leaderBoardData)
        {
            _setLeaderBoardTableService.SetLeaderBoardTableV2(leaderBoardData);
        }

        public IEnumerable<LeaderBoardModelV2> GetLeaderBoardDataV2(int is14, int siteId, string eventCode, string startDate, string endDate, string timeZone, string sPName)
        {
            return _setLeaderBoardTableService.GetLeaderBoardDataV2(is14, siteId, eventCode, startDate, endDate, timeZone, sPName);
        }

        public IEnumerable<LeaderBoardModelV2> GetLeaderBoardExtraPoint( int siteId, string eventCode, string startDate, string endDate, string timeZone ,string sPName)
        {
            return _setLeaderBoardTableService.GetLeaderBoardExtraPoint(siteId, eventCode, startDate, endDate, timeZone, sPName);
        }

        public IEnumerable<LeaderBoardEvent> GetLeaderboardEvent()
        {
            return _setLeaderBoardTableService.GetLeaderboardEvent();
        }

        public void CheckLeaderboardOverTime()
        {
            _setLeaderBoardTableService.CheckLeaderboardOverTime();
        }

    }
}