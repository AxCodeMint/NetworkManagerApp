using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using NetworkManagerApp.Interfaces;
using NetworkManagerApp.Models;

namespace NetworkManagerApp.Services
{
    public class NetworkConfigurationService : INetworkConfigurationService
    {
        private const string _dnsRegexPattern = @"^((25[0-5]|2[0-4][0-9]|1?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|1?[0-9][0-9]?)$";
        private const string _iPv4RegexPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        public List<NetworkInterfaceModel> GetNetworkInterfaces()
        {
            var interfaces = new List<NetworkInterfaceModel>();

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var model = new NetworkInterfaceModel
                {
                    Name = netInterface.Name,
                    Type = netInterface.NetworkInterfaceType.ToString(),
                    Status = netInterface.OperationalStatus.ToString(),
                    MacAddress = netInterface.GetPhysicalAddress().ToString()
                };


                foreach (var ip in netInterface.GetIPProperties().UnicastAddresses)
                {

                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork) //ipv4
                    {
                        model.IPv4Addresses.Add(new()
                        {
                            Address = ip.Address.ToString(),
                            SubnetMask = ip.IPv4Mask.ToString(),
                            Gateway = "",
                        });
                    }
                    else if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        model.IPv6Addresses.Add(ip.Address.ToString());
                    }
                }

                var gatewayAddresses = netInterface.GetIPProperties().GatewayAddresses;
                if (gatewayAddresses.Any())
                {
                    model.Gateway = gatewayAddresses.First().Address.ToString();
                }

                var dnsAddresses = netInterface.GetIPProperties().DnsAddresses;
                if (dnsAddresses.Any())
                {
                    model.DNS = dnsAddresses.First().ToString();
                }

                interfaces.Add(model);
            }
            return interfaces;
        }

        private bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private bool ExecuteCommand(string command)
        {
            if (!IsWindows() && !IsLinux())
                return false;

            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = IsWindows() ? "cmd.exe" : "/bin/bash",
                    Arguments = IsWindows() ? $"/C {command}" : $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processInfo);
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public string GetCurrentIPv4Address(string interfaceName)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.Name == interfaceName);

            if (networkInterface != null)
            {
                var ipAddress = networkInterface.GetIPProperties().UnicastAddresses
                    .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString();

                if (ipAddress == null || ipAddress == "")
                {
                    return string.Empty;
                }

                return ipAddress;
            }

            return string.Empty;
        }

        public bool SetIPv4Address(string ipv4Address, string networkInterfaceName)
        {
            if (!Regex.IsMatch(ipv4Address, _iPv4RegexPattern))
                return false;

            string command = IsWindows()
                ? $"netsh interface ipv4 set address \"{networkInterfaceName}\" static {ipv4Address}"
                : $"nmcli con mod {networkInterfaceName} ipv4.addresses {ipv4Address} && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool RemoveIPv4Address(string ipv4Address, string networkInterfaceName)
        {
            if (!Regex.IsMatch(ipv4Address, _iPv4RegexPattern))
                return false;

            string command = IsWindows()
                ? $"netsh interface ipv4 delete address \"{networkInterfaceName}\" {ipv4Address}"
                : $"nmcli con mod {networkInterfaceName} -ipv4.addresses {ipv4Address} && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool AddIPv4Address(string ipv4Address, string networkInterfaceName)
        {
            if (!Regex.IsMatch(ipv4Address, _iPv4RegexPattern))
                return false;

            string command = IsWindows()
                ? $"netsh interface ipv4 add address \"{networkInterfaceName}\" {ipv4Address} 255.255.255.0"
                : $"nmcli con mod {networkInterfaceName} ipv4.addresses {ipv4Address} && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool SetIPv4Automatic(string networkInterfaceName)
        {
            string command = IsWindows()
                ? $"netsh interface ipv4 set address \"{networkInterfaceName}\" dhcp"
                : $"nmcli con mod {networkInterfaceName} ipv4.method auto && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool SetGateway(string ipv4Address, string subnetMask, string gateway, string networkInterfaceName)
        {
            if (!Regex.IsMatch(ipv4Address, _iPv4RegexPattern))
            {
                return false;
            }

            if (subnetMask == null || subnetMask == "")
            {
                subnetMask = "255.255.255.0";
            }

            string command = IsWindows()
                ? $"netsh interface ipv4 set address name=\"{networkInterfaceName}\" static {ipv4Address} {subnetMask} {gateway}"
                : $"nmcli con mod {networkInterfaceName} ipv4.gateway {gateway} && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool SetGatewayAutomatic(string networkInterfaceName)
        {
            string command = IsWindows()
                ? $"netsh interface ipv4 set address \"{networkInterfaceName}\" dhcp"
                : $"nmcli con mod {networkInterfaceName} ipv4.method auto && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool SetDNS(string primaryDNS, string networkInterfaceName)
        {
            if (!Regex.IsMatch(primaryDNS, _dnsRegexPattern))
                return false;

            string command = IsWindows()
                ? $"netsh interface ip set dns \"{networkInterfaceName}\" static {primaryDNS}"
                : $"nmcli con mod {networkInterfaceName} ipv4.dns {primaryDNS} && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

        public bool SetDNSAutomatic2(string networkInterfaceName)
        {
            try
            {
                string command = $"netsh interface ip set dns \"{networkInterfaceName}\" source=dhcp ";
                var process = Process.Start(new ProcessStartInfo("cmd.exe", $"/C {command}") { CreateNoWindow = true, UseShellExecute = false });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SetDNSAutomatic(string networkInterfaceName)
        {
            string command = IsWindows()
                ? $"netsh interface ip set dns \"{networkInterfaceName}\" source=dhcp"
                : $"nmcli con mod {networkInterfaceName} ipv4.ignore-auto-dns no && nmcli con up {networkInterfaceName}";

            return ExecuteCommand(command);
        }

    }
}
