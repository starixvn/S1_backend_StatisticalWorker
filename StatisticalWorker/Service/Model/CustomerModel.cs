using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StatisticalWorker.Service.Model
{
    public class DormantSettingInput
    {
        public int SiteId { get; set; }
    }

    public class DormantSettingOutput
    {
        public int SiteId { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public string Setting { get; set; }
        public IEnumerable<int> Days => JsonConvert.DeserializeObject<DormantSetting>(Setting).Days ;
    }

    public class DormantSetting
    {
        public List<int> Days { get; set; }
    }

    public class DormantAccountInput
    {
        public int SiteId { get; set; }
        public int Day { get; set; }
    }
    
    public class DormantAccountOutput
    {
        public int CustId { get; set; }
        public string UserName { get; set; }
        public int SiteId { get; set; }
        public DateTime LastLockTime { get; set; }
    }

    public class LockDormantAccountInput
    {
        public int CustId { get; set; }
        public int Status { get; set; }
    }
    
    public class CustNotesInput
    {
        public int CustId { get; set; }
        public string Note { get; set; }
        public string Operator { get; set; }
    }
}