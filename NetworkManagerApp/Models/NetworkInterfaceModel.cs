using NetworkManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace NetworkManagerApp.Models
{
    public class NetworkInterfaceModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? MacAddress { get; set; }
        public List<Ipv4Model> IPv4Addresses { get; set; } = new List<Ipv4Model>();
        public List<string> IPv6Addresses { get; set; } = new List<string>();
        public string? SubnetMask { get; set; }
        public string? Gateway { get; set; }

        public string? DNS { get; set; }
    }
}
