using ASEDependencyCheck.Model;
using ASEDependencyCheck.Services.Interfaces;
using CommandDotNet;
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

        }

        private async Task<bool> LoadJsonFile()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            try
            {
                string jsonString = await File.ReadAllTextAsync("endpoints.json");
                _endpoints = JsonSerializer.Deserialize<Endpoints>(jsonString, options);

                return true;
                
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Unable to find endpoints.json file");
                return false;
            }
            

            
        }

        [Command(Name = "test-connectivity",
        Usage = "get-token -e endpoints.json",
        Description = "Gets a token for the specified resource")]
        public async Task TestConnectivity(
        [Option(LongName = "endpoint", ShortName = "e",
        Description = "location of endpoint.json file which contains the list of endpoints to tcpping ")]
        string endpointFileName)
        {
            string hostName; int port, maxTries = 4;
            // 1. Validate the endpoint
            if (endpointFileName.Contains(".json"))
            {
                bool fileLoadSuccessful = await LoadJsonFile();
                if(fileLoadSuccessful)
                {
                    foreach (string dependency in _endpoints.dependencies)
                    {
                        string[] temp =  dependency.Split(":");
                        if(temp.Length == 1)
                        {
                            hostName = temp[0];
                            port = 80;
                        }
                        else
                        {
                            hostName = temp[0];
                            port = Convert.ToInt32(temp[1]);
                        }

                        await _conn.Tcpping(hostName, port, maxTries);

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
