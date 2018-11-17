using System.Net;

namespace JT.Infrastructure.AppCommon
{
    public class SystemInfo
    {
        /// <summary>
        /// Get HostName
        /// </summary>
        public static string LocalHostName
        {
            get
            {
                return Dns.GetHostName();
            }
        }

        /// <summary>
        /// Get localhost IP address(IPv4)
        /// </summary>
        public static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ips = Dns.GetHostEntry(hostName).AddressList;
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;
                return ip.ToString();
            }
            return string.Empty;

            //if (HttpContext.Current != null)
            //{
            //    string userIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //    if (userIP == null || userIP == "")
            //    {
            //        //没有代理服务器,如果有代理服务器获取的是代理服务器的IP
            //        userIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //    }
            //    return userIP;
            //}
        }
    }
}
