using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Models
{
    public class RequestLoadData
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public Dictionary<string, string> Filters { get; set; }

    }
}
