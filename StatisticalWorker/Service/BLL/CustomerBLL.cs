using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StatisticalWorker.Service.DAL;
using StatisticalWorker.Service.Extensions;
using StatisticalWorker.Service.Model;
using StatisticalWorker.Service.Model.enums;

namespace StatisticalWorker.Service.BLL
{
    public class CustomerBLL
    {
        private readonly CustomerService _customerService;
        private readonly SelLangBLL _selLangBll;
        private readonly ILogger<CustomerBLL> _logger;
        private const string Operator = "System";
        private const string Language = "en";

        public CustomerBLL(IServiceProvider serviceProvider)
        {
            _customerService = serviceProvider.GetRequiredService<CustomerService>();
            _selLangBll = serviceProvider.GetRequiredService<SelLangBLL>();
            _logger = serviceProvider.GetRequiredService<ILogger<CustomerBLL>>();
        }
        
        public bool ProcessCustomerStatus()
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                var dormantSettings = GetDormantSetting();
                foreach (var dormantSetting in dormantSettings)
                {
                    if ((currentTime - dormantSetting.LastUpdateTime).TotalDays < 1)
                    {
                        continue;
                    }
                    
                    foreach (var day in dormantSetting.Days)
                    {
                        var statusList = new List<int> {(int) CustStatus.Running};
                        var unDepositCustomers = GetDormantAccount(new DormantAccountInput
                        {
                            SiteId = dormantSetting.SiteId,
                            Day = day,
                            StatusList = $"[{string.Join(",", statusList)}]"
                        });

                        foreach (var unDepositCustomer in unDepositCustomers)
                        {
                            if ((currentTime - unDepositCustomer.LastLockTime).TotalDays > 60)
                            {
                                var result = LockDormantAccountSet(unDepositCustomer.CustId, day);
                                if (result >= 0)
                                {
                                    CustNotesSet(new CustNotesInput
                                    {
                                        CustId = unDepositCustomer.CustId,
                                        Note = string.Format(
                                            _selLangBll.GetResourceValue("lbl_NoneDepositAndNoneBet", Language),
                                            day.ToString()),
                                        Operator = Operator
                                    });
                                }
                            }
                        }
                    }

                    DormantSettingSet(new DormantSettingInput {SiteId = dormantSetting.SiteId});
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodExtension.GetCurrentMethod());
                return false;
            }
        }
        
        private IEnumerable<DormantSettingOutput> GetDormantSetting()
        {
            return _customerService.GetDormantSetting();
        }
        
        private IEnumerable<DormantAccountOutput> GetDormantAccount(DormantAccountInput dormantAccountInput)
        {
            return _customerService.GetDormantAccount(dormantAccountInput);
        }
        
        private int DormantSettingSet(DormantSettingInput dormantSettingInput)
        {
            return _customerService.DormantSettingSet(dormantSettingInput);
        }
        
        private int LockDormantAccountSet(int custId, int day)
        {
            return _customerService.LockDormantAccountSet(new LockDormantAccountInput
            {
                CustId = custId,
                Status = (int)CustStatus.Lock
            });
        }

        private int CustNotesSet(CustNotesInput custNotesInput)
        {
            return _customerService.CustNotesSet(custNotesInput);
        }
    }
}