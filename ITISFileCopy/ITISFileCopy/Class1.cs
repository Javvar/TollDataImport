using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ITISFileCopy
{
    public static class RemoteServerLogon
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogonUser(
            string userName,
            string domain,
            string password,
            LogonType logonType,
            LogonProvider logonProvider,
            out IntPtr token);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateToken(
            IntPtr existingTokeHandle,
            SecurityImpersonationLevel securityImpersonationLevel,
            out IntPtr duplicateTokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);


        public enum SecurityImpersonationLevel : int
        {
            Anonymous = 0,
            Identification = 1,
            Impersonation = 2,
            Delegation = 3
        }

        public enum LogonProvider : int
        {
            Default = 0,
            WinNT35 = 1,
            WinNT40 = 2,
            WinNT50 = 3
        }

        public enum LogonType : int
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8,
            NewCredentials = 9
        }
    }
}
