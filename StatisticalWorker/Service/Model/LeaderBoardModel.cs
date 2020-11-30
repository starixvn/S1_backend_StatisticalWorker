using System;

namespace StatisticalWorker.Service.Model
{
    public class LeaderBoardModel
    {
        public DateTime Date { get; set; }
        public int LBCategoryId { get; set; }
        public int CustId { get; set; }
        public int SiteId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Turnover { get; set; }
        public int TurnoverSiteRank { get; set; }
        public int TurnoverCategoryRank { get; set; }
    }
}
