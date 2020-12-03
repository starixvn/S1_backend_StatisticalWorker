using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.Model
{
    public class OnlinePopulationModel
    {
        public int SiteId { get; set; }
        public int Count { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
