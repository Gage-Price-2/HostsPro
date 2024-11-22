using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.BussinessServices
{
    internal class IpLookupManager
    {
        public async Task<string> GetIPAddressAsync(string dnsName)
        {
            string output = string.Empty;
            await Task.Run(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo("nslookup", dnsName)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    output = process.StandardOutput.ReadToEnd();
                }
            });

            // Simple parsing logic to find the IP address in the output
            string[] lines = output.Split('\n');
            foreach (string line in lines)
            {
                if (line.Contains("Address:"))
                {
                    return line.Split("Address:")[1].Trim();
                }
            }

            return string.Empty;
        }
    }
}
