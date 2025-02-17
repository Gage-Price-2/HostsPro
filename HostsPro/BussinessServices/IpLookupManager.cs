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
        /// <summary>
        /// Async method to resolve a locaiton to an IP address 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="timeout"></param>
        /// <returns>string IP address</returns>
        public async Task<string> ResolveDnsWithTimeoutAsync(string hostname, TimeSpan timeout)
        {
            //CancellationTokenSource is used to end a task
            using var cts = new CancellationTokenSource();

            //get the helper method definition as a variable to be called with a timeout delay
            Task<string> dnsLookupTask = ResolveDnsAsync(hostname);

            //Call helper method with timeout
            Task completedTask = await Task.WhenAny(dnsLookupTask, Task.Delay(timeout, cts.Token));

            if (completedTask == dnsLookupTask)
            {
                // If lookup finished in time, cancel the delay task
                cts.Cancel();
                return await dnsLookupTask;
            }

            // If lookup didn't finish in time, return null which triggers error message
            return null;
        }
        /// <summary>
        /// Helper method to do the actual lookup
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        private async Task<string> ResolveDnsAsync(string hostname)
        {
            try
            {
                //Built in system method to get IP address
                var hostEntry = await Dns.GetHostEntryAsync(hostname);
                foreach (var ip in hostEntry.AddressList)
                {
                    //checks if IPv4
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) 
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception)
            {
                //Used to catch possible error, returns null after leaves catch
            }
            return null;
        }
    }
}
