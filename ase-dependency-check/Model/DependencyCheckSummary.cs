using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Model
{
    public class DependencyCheckSummary
    {
        public int totalDependenciesCalled { get; set; }
        public int totalDependenciesFailed { get; set; }
        public string nextSteps { get; set; }
        public List<string> endpointsFailed { get; set; }
    }
}
