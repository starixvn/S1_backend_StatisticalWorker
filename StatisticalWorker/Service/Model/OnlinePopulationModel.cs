using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.Model
{
    public class OnlineCountModel
    {
        public int SiteId { get; set; }
        public byte DeviceType { get; set; }
        public int Count { get; set; }
    }
}
