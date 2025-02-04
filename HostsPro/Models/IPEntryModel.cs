using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.Models
{
    public class IPEntryModel
    {
        public string IpAddress { get; set; } = string.Empty;
        public string DNS { get; set; } = string.Empty;

        public string RoutesTo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string Comment { get; set; } = string.Empty;
    }
}
