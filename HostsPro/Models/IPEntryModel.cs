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
        public string? IpAddress { get; set; }
        public string? DNS { get; set; }

        public string? RoutesTo { get; set; }
        public bool? IsActive { get; set; }
        public string? Comment { get; set; }
    }
}
