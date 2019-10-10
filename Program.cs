using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            List<string> addrList = new List<string>();

            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].ToString().Contains("."))
                {
                    System.Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
                    addrList.Add("https://" + addr[i].ToString());
                }
            }
            CreateHostBuilder(addrList.ToArray());
        }

        private static void CreateHostBuilder(string[] addrList)
        {
            IWebHost host = new WebHostBuilder()
                .UseUrls(addrList)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
