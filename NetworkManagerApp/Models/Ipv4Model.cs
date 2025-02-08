using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManagerApp.Models
{
    public class Ipv4Model
    {
        public string Address { get; set; } = string.Empty;
        public string SubnetMask { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;

        public override string ToString()
        {
            return Address;
        }
    }
}
