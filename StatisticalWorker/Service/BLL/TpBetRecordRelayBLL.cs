using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.BLL
{
    public class TpBetRecordRelayBLL
    {
        #region property

        private readonly TpBetRecordRelayService _tpBetRecordRelayService;
        
        #endregion

        #region Construction method

        public TpBetRecordRelayBLL(TpBetRecordRelayService tpBetRecordRelayService)
        {
            _tpBetRecordRelayService = tpBetRecordRelayService;
        }

        #endregion

        #region method

        /// <summary>
        /// Select data and Insert its
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Affected row counts</returns>
        public int SetTpBetRecordRelayForReport(TpBetRecordRelayForReportParameterModel parameter)
        {
            return _tpBetRecordRelayService.SetTpBetRecordRelayData(parameter);
        }

        #endregion
    }
}
