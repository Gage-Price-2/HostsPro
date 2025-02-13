using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.BussinessServices
{
    internal class IpLookupManager
    {
        //public async Task<string> GetIPAddressAsync(string dnsName)
        //{
        //    string output = string.Empty;
        //    await Task.Run(() =>
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo("nslookup", dnsName)
        //        {
        //            RedirectStandardOutput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        using (Process process = Process.Start(psi))
        //        {
        //            output = process.StandardOutput.ReadToEnd();
        //        }
        //    });

        //    // Simple parsing logic to find the IP address in the output
        //    string[] lines = output.Split('\n');
        //    foreach (string line in lines)
        //    {
        //        if (line.Contains("Address:"))
        //        {
        //            return line.Split("Address:")[1].Trim();
        //        }
        //    }

        //    return string.Empty;
        //}

        public async Task<string> ResolveDnsWithTimeoutAsync(string hostname, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            Task<string> dnsLookupTask = ResolveDnsAsync(hostname);

            Task completedTask = await Task.WhenAny(dnsLookupTask, Task.Delay(timeout, cts.Token));

            if (completedTask == dnsLookupTask)
            {
                // If lookup finished in time, cancel the delay task
                cts.Cancel();
                return await dnsLookupTask;
            }

            // If lookup didn't finish in time, return null (triggers error message)
            return null;
        }
        private async Task<string> ResolveDnsAsync(string hostname)
        {
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(hostname);
                foreach (var ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4 Only
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception)
            {
                // Lookup failed, return null
            }
            return null;
        }
    }
}
