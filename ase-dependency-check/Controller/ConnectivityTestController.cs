using ASEDependencyCheck.Model;
using ASEDependencyCheck.Services.Interfaces;
using CommandDotNet;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASEDependencyCheck.Controller
{
    public class ConnectivityTestController
    {
        private readonly ISocketConnection _conn;
        private readonly ILogger<ConnectivityTestController> _logger;
        private Endpoints _endpoints;

        public ConnectivityTestController(ISocketConnection conn, ILogger<ConnectivityTestController> logger)
        {
            _conn = conn?? throw new ArgumentNullException(nameof(conn));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            string version = "v1.0.0.0";
            _logger.LogInformation($"Running ASE-Dependency Checker {version}");
            _logger.LogInformation("");

        }

        private async Task<bool> LoadJsonFile(string endptfilename)
        {
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            try
            {
                string jsonString = await File.ReadAllTextAsync(endptfilename);
                _endpoints = JsonSerializer.Deserialize<Endpoints>(jsonString, options);

                return true;
                
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Unable to find endpoints.json file");
                return false;
            }
        }

        private async Task CheckDependency(List<string> dependencies)
        {
            string hostName;
            int port, i=1, maxTries = 4;
            string[] temp;

            DependencyCheckSummary dependencysummary = new DependencyCheckSummary();
            // ds.totalDependenciesCalled = _endpoints.dependencies.Count;
            dependencysummary.totalDependenciesCalled = dependencies.Count;
            dependencysummary.totalDependenciesFailed = 0;

            foreach (string dependency in dependencies)
            {
                temp = dependency.Split(":");
                if (temp.Length == 1)
                {
                    hostName = temp[0];
                    port = 80;
                }
                else
                {
                    hostName = temp[0];
                    port = Convert.ToInt32(temp[1]);
                }
                _logger.LogInformation($"Testing connectivity to endpoint {i}/{dependencies.Count} - {hostName}:{port} for {maxTries} times");
                await _conn.Tcpping(hostName, port, maxTries);

                TcppingErrorSummary tcpsummary = await _conn.Tcpping(hostName, port, maxTries);
                if (tcpsummary.successRate != 100)
                {
                    dependencysummary.totalDependenciesFailed++;
                    dependencysummary.endpointsFailed.Add(tcpsummary.hostName + ":" + tcpsummary.port.ToString());
                }
                i++;

                _logger.LogInformation("");
                _logger.LogInformation("");
            }
        }

        [Command(Name = "test-connectivity",
        Usage = "test-connectivity -e endpoints.json -p platformType",
        Description = "Tests connectivity to all endpoints in endpoints.json based on platformType")]
        public async Task TestConnectivity(
        [Option(LongName = "endpoint", ShortName = "e",
        Description = "location of endpoint.json file which contains the list of endpoints to tcpping ")]
        string endpointFileName, [Option(LongName = "platformType", ShortName = "p",
        Description = "Indicates the platform type (windows/linux) to target dependency checks")]
        string platformType)
        {
            // 1. Validate the endpoint
            if (endpointFileName.Contains(".json"))
            {
                bool fileLoadSuccessful = await LoadJsonFile(endpointFileName);
                if(fileLoadSuccessful)
                {
                    _logger.LogInformation("Loaded Json file with Endpoints.");
                    _logger.LogInformation("");

                    if (platformType == "all")
                    {
                        await CheckDependency(_endpoints.dependencies["windowsdependencies"]);
                        await CheckDependency(_endpoints.dependencies["linuxdependencies"]);
                        
                    }
                    else if (platformType == "linux" || platformType == "windows")
                    {
                        await CheckDependency(_endpoints.dependencies[$"{platformType}dependencies"]);
                    }
                    else
                    {
                        _logger.LogError("Invalid platformType value. Allowed values 'windows' or 'linux' or 'all'");
                    }
                }
            }
            else
            {
                _logger.LogError("Invalid File extenstion entered. Please provide json as input");
            }
        }
    }
}