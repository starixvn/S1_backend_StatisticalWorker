using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.BLL
{
    public class CustClosingBalanceBLL
    {
        #region property

        private readonly CustClosingBalanceService _custClosingBalanceService;

        #endregion

        #region Construction method

        public CustClosingBalanceBLL(CustClosingBalanceService custClosingBalanceService)
        {
            _custClosingBalanceService = custClosingBalanceService;
        }

        #endregion

        #region method

        /// <summary>
        /// Select data and Insert its
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Affected row counts</returns>
        public int SetCustClosingBalance(CustClosingBalanceParameterModel parameter)
        {
            return _custClosingBalanceService.SetCustClosingBalanceData(parameter);
        }

        #endregion
    }
}
