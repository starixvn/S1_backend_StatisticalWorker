using StatisticalWorker.Service.DAL;

namespace StatisticalWorker.Service.BLL
{
    public class CustClosingBalanceBLL
    {
        private readonly CustClosingBalanceService _custClosingBalanceService;

        public CustClosingBalanceBLL(IServiceProvider provider)
        {
            _custClosingBalanceService = provider.GetRequiredService<CustClosingBalanceService>();
        }

        public int SetCustClosingBalance()
        {
            return _custClosingBalanceService.SetCustClosingBalanceData();
        }
    }
}
