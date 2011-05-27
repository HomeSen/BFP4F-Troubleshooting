using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;

namespace BFP4F_Troubleshooting
{
    class NetworkHelper
    {
        #region Fields

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

        const string URL_PBSVC = "http://www.evenbalance.com/downloads/pbsvc/pbsvc.exe";
        const int BUFFER_SIZE = 262144; // 256 KiB
        
        #endregion


        #region Ping

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

        #endregion


        #region Downloads

        public static bool DownloadPbSvc(string target)
        {
            bool success = false;
            if (File.Exists(target))
                File.Delete(target);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL_PBSVC);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
                WebResponse response = request.GetResponse();

                BinaryReader sr = new BinaryReader(response.GetResponseStream());
                BinaryWriter sw = new BinaryWriter(new StreamWriter(target).BaseStream);

                long bytesReceived = 0;

                do
                {
                    byte[] data;
                    if ((BUFFER_SIZE + bytesReceived) <= response.ContentLength)
                        data = sr.ReadBytes(BUFFER_SIZE);
                    else
                        data = sr.ReadBytes((int)(response.ContentLength - bytesReceived));

                    bytesReceived += BUFFER_SIZE;
                    sw.Write(data);
                    sw.Flush();
                } while (bytesReceived < response.ContentLength);

                sw.Close();
                sr.Close();

                success = File.Exists(target);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "DownloadPbSvc()");
                success = false;
            }

            return success;
        }

        #endregion
    }
}
