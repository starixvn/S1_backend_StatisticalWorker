using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.Model
{
    public class TpBetRecordRelayForReportModel
    {
        public string Time { get; set; }

        public int TpId { get; set; }

        public int GameType { get; set; }

        public int SiteId { get; set; }

        public int CurrencyId { get; set; }

        public int CustId { get; set; }

        public decimal StakeSummary { get; set; }

        public decimal ValidBetSummary { get; set; }

        public decimal WinLossSummary { get; set; }
    }
}
