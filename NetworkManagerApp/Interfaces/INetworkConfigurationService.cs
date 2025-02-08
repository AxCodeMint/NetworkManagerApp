using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkManagerApp.Models;

namespace NetworkManagerApp.Interfaces
{
    public interface INetworkConfigurationService
    {
        List<NetworkInterfaceModel> GetNetworkInterfaces();
        string GetCurrentIPv4Address(string interfaceName);
        bool SetIPv4Address(string ipv4Address, string networkInterfaceName);
        bool RemoveIPv4Address(string ipv4Address, string networkInterfaceName);
        bool AddIPv4Address(string ipv4Address, string networkInterfaceName);
        bool SetIPv4Automatic(string networkInterfaceName);
        bool SetDNS(string primaryDNS, string networkInterfaceName);
        bool SetDNSAutomatic(string networkInterfaceName);
        bool SetGateway(string ipv4Address, string subnetMask, string gateway, string networkInterfaceName);
        bool SetGatewayAutomatic(string networkInterfaceName);
    }
}
