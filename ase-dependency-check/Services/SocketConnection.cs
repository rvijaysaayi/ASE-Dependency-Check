using ASEDependencyCheck.Model;
using ASEDependencyCheck.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Services
{
    public class SocketConnection : ISocketConnection
    {
        private readonly ILogger<SocketConnection> _logger;

        public SocketConnection(ILogger<SocketConnection> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        async Task<TcppingErrorSummary> ISocketConnection.Tcpping(string host, int port, int maxTries)
        {
            TcppingErrorSummary tcpSummary = new TcppingErrorSummary();
            tcpSummary.hostName = host;
            tcpSummary.port = port;

            int success = 0;
            for (int i = 0; i < maxTries; i++)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Blocking = true;

                Stopwatch stopwatch = new Stopwatch();
                
                try
                {
                    stopwatch.Start();
                    await sock.ConnectAsync(host, port);
                    stopwatch.Stop();
                    success++;
                    _logger.LogInformation($"Connected to {host}:{port} :" + "{0:0.00} ms", stopwatch.Elapsed.TotalMilliseconds);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    tcpSummary.message = ex.Message;
                }
            
                sock.Close();
                Thread.Sleep(1000);
            }

              tcpSummary.successRate = success / maxTries * 100;

            _logger.LogInformation($"tcpping statistics for {host}:{port}:");
            _logger.LogInformation($"Packets Sent = {maxTries}, Received = {success} , Loss = {maxTries-success} ({(maxTries-success)/maxTries * 100}% Loss)");

         return tcpSummary;
        }
    }
}