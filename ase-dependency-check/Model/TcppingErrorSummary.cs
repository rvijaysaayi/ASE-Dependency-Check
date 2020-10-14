using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Model
{
    public class TcppingErrorSummary
    {
        public string hostName { get; set; }
        public int port { get; set; }
        public string message { get; set; }
        public int successRate { get; set; }
    }
}
