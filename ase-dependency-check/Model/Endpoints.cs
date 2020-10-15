using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Model
{
    public class Endpoints
    {
        public string version { get; set; }
        public Dictionary<string, List<string>> dependencies { get; set; }
    }
}