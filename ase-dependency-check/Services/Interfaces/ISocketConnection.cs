using ASEDependencyCheck.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Services.Interfaces
{
    public interface ISocketConnection
    {
        Task<TcppingErrorSummary> Tcpping(string host, int port, int maxTries);
    }
}