using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intertoll.PCS.DataIntergrationService.Managers
{
    public static class NetworkUtility
    {
        public static bool CanPingHost(string ipAddress, Action<string> failureCallback)
        {
            var success = false;

            var buffer = new byte[32];
            var pingOptions = new PingOptions(128, true);

            try
            {
                var pingReply = new Ping().Send(ipAddress, 10000, buffer, pingOptions);

                if (pingReply == null)
                    failureCallback("Ping failed: ping reply null");
                else
                {
                    switch (pingReply.Status)
                    {
                        case IPStatus.Success:
                            success = true;
                            break;
                        default:
                            failureCallback(string.Format("Ping failed, ping reply status: {0}", pingReply.Status));
                            break;
                    }
                }

            }
            catch (PingException e)
            {
                success = false;
                var code = (SystemExceptionHResult)Marshal.GetHRForException(e);

                switch (code)
                {
                    case SystemExceptionHResult.NoSuchHostIsKnown:
                        failureCallback(string.Format("No such host is known '{0}'.", ipAddress));
                        break;
                    default:
                        failureCallback(string.Format("Ping failed, exception: {0}", e.StackTrace));
                        break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            return success;
        }
    }
    enum SystemExceptionHResult
    {
        NoSuchHostIsKnown = -2146233079
    }
}
