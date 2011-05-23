using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace BFP4F_Troubleshooting
{
    class NetworkHelper
    {
        public static string[] servers = new string[]{ "321082-gosprapp106.ea.com",
                                                        "gosredirector.ea.com",
                                                        "gossjcprod-qos01.ea.com", 
                                                        "gosiadprod-qos01.ea.com",
                                                        "gosnrtprod-qos01.ea.com",
                                                        "gosgvaprod-qos01.ea.com",
                                                        "cdn.battlefield.play4free.com" };
        public static string[] serverIPs = new string[]{ "94.236.97.4",
                                                        "159.153.235.22",
                                                        "159.153.202.54", 
                                                        "159.153.105.104",
                                                        "159.153.174.133",
                                                        "159.153.161.178",
                                                        "195.95.193.78" };

        public static long PingServer(string server)
        {
            long result = -1;
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(server, 1000);
                if (reply.Status == IPStatus.Success)
                    result = reply.RoundtripTime;
                else if (reply.Status == IPStatus.TimedOut)
                    result = 0;
                else
                    result = -1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "PingServer()");
            }

            return result;
        }

        public static long PingServer(IPAddress server)
        {
            long result = -1;
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(server, 1000);
                if (reply.Status == IPStatus.Success)
                    result = reply.RoundtripTime;
                else
                    result = -1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "PingServer()");
            }

            return result;
        }
    }
}
