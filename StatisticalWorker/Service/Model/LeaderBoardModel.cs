using System;

namespace StatisticalWorker.Service.Model
{
    public class LeaderBoardModel
    {
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public int CustId { get; set; }
        public int SiteId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Turnover { get; set; }
        public int TurnoverSiteRank { get; set; }
        public int TurnoverCategoryRank { get; set; }
        public decimal WinlostAmount { get; set; }
    }
    public class DepositModel
    {
        public DateTime Date { get; set; }
        public int CustId { get; set; }
        public int SiteId { get; set; }
        public int CurrencyId { get; set; }
        public decimal ActualDeposit { get; set; }
    }
    public class LeaderBoardModelV2
    {
        public DateTime Date { get; set; }
        public string EventCode { get; set; }
        public int CategoryId { get; set; }
        public int CustId { get; set; }
        public int SiteId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Turnover { get; set; }
        public int TurnoverSiteRank { get; set; }
        public int TurnoverCategoryRank { get; set; }
        public decimal WinlostAmount { get; set; }
    }
    public class LeaderBoardEvent
    {
        public string EventCode { get; set; }
        public int SiteId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TimeZoom { get; set; }
        public string SPName { get; set; }
        public string ExtraPointSP { get; set; }
    }
}
