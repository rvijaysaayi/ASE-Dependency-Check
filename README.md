# ASE Dependency Check
The Azure App Service Environment is an Azure App Service feature that provides a fully isolated and dedicated environment for securely running App Service apps at high scale.

The "ASE Dependency Check tool" helps in troubleshooting and isolating issues when the Network Virtual Appliance (NVA) is blocking outbound traffic from the subnet where ASE is deployed. 

## Prerequisites:
The ASE has dependency on few ports and these should be allowed to allowed for the functioning of the ASE.
 
### Inbound :
<ul>
    <li>from the IP service tag AppServiceManagement on ports 454,455</li>
    <li>from the load balancer on port 16001</li>
    <li>from the ASE subnet to the ASE subnet on all ports</li>
</ul>
    
### Outbound :
<ul>
    <li>to all IPs on port 123</li>
    <li>to all IPs on ports 80, 443</li>
    <li>to the IP service tag AzureSQL on ports 1433</li>
    <li>to all IPs on port 12000</li>
    <li>to the ASE subnet on all ports</li>
</ul>
	 
The DNS port does not need to be added as traffic to DNS is not affected by NSG rules. These ports do not include the ports that your apps require for successful use. The normal app access ports are:

| Use                               | Ports             |
|-----------------------------------|-------------------|
| HTTP/HTTPS                        |80 , 443           |
| FTP/FTPS                          |21,990,1001-10020  |
| Visual Studio remote debugging    |4020,4022,4024     |
| Web Deploy service                |8172               |

## Installation Steps:
### A) Troubleshoot issues with ASE deployment failure :
1. Deploy a Azure Virtual Machine in the Azure Virtual Network's subnet where the ASE was planned to be deployed.
2. Navigate to [Download latest version](https://github.com/vijaysaayi/ASE-Dependency-Check/tree/main/Download) and download the zip file.
3. Extract the zip file.
4. Run the exe using the command shown below.
5. A report would be generated once the tests are completed.
### B) Troubleshoot issue with connectivity from App Services deployed in ASE. 
1. Navigate to Kudu Console of App Service
2. Navigate to [Download latest version](https://github.com/vijaysaayi/ASE-Dependency-Check/tree/main/Download) and download the zip file.
3. Extract the zip file in your local machine and drag and drop the .exe and .json files to any folder in Kudu Console of the App Service.
4. Run the exe using the command shown below.
5. A report would be generated once the tests are completed.

## Commands:
> ```bash
> ASEDependencyCheck test-connectivity -e <path to endpoints.json file> -p <platform type>
> ```

> Supported value for platform type are all, windows, linux

## Sample Report :

  
    
