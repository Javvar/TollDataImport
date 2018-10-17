using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intertoll.NLogger;

namespace ITISFileCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                DateTime FilesDate = DateTime.Today.AddDays(-1);

                if (ConfigurationManager.AppSettings["Date"] != null && ConfigurationManager.AppSettings["Date"] != string.Empty)
                    FilesDate = DateTime.ParseExact(ConfigurationManager.AppSettings["Date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var AvcSrcPath = ConfigurationManager.AppSettings["AvcSrcPath"];
                var TrfSrcPath = ConfigurationManager.AppSettings["TrfSrcPath"];
                var TgtPath = ConfigurationManager.AppSettings["TgtPath"];

                var VirtualPlazas = ConfigurationManager.AppSettings["VirtualPlazas"].Split(';');

                IntPtr token = new IntPtr();

                foreach (var svr in ConfigurationManager.AppSettings["Servers"].Split(';'))
                {
                    if (!RemoteServerLogon.LogonUser("username", svr, "password", RemoteServerLogon.LogonType.NewCredentials, RemoteServerLogon.LogonProvider.Default, out token))
                    {
                        Log.LogTrace("Could not logon to " + svr);
                    }
                }

                try
                {
                    IntPtr tokenDuplicate;

                    if (!RemoteServerLogon.DuplicateToken(token, RemoteServerLogon.SecurityImpersonationLevel.Impersonation, out tokenDuplicate))
                    {
                        Log.LogTrace("Could not duplicate");
                    }

                    try
                    {
                        using (WindowsImpersonationContext context = new WindowsIdentity(tokenDuplicate).Impersonate())
                        {
                            while (FilesDate < DateTime.Today)
                            {
                                foreach (var vp in VirtualPlazas)
                                {
                                    var avcfileName = string.Format("{0}_{1}_{2}_{3}.avc", vp, FilesDate.Year, FilesDate.Month.ToString("d2"), FilesDate.Day.ToString("d2"));
                                    var avcfileSrcPath = AvcSrcPath + avcfileName;
                                    var avcfileTgtPath = TgtPath + avcfileName;


                                    var trffileName = string.Format("{0}_{1}_{2}_{3}.trf", vp, FilesDate.Year, FilesDate.Month.ToString("d2"), (FilesDate.AddDays(-1).Day).ToString("d2"));
                                    var trffileSrcPath = TrfSrcPath + trffileName;
                                    var trffileTgtPath = TgtPath + trffileName;

                                    if (!File.Exists(avcfileSrcPath))
                                    {
                                        Console.WriteLine("file " + avcfileSrcPath + " does not exist");
                                    }
                                    else if (File.Exists(avcfileTgtPath))
                                    {
                                        Console.WriteLine("file " + avcfileTgtPath + " already copied");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Copying " + avcfileSrcPath + " to " + avcfileTgtPath);
                                        File.Copy(avcfileSrcPath, avcfileTgtPath);
                                    }

                                    if (!File.Exists(trffileSrcPath))
                                    {
                                        Console.WriteLine("file " + trffileSrcPath + " does not exist");
                                    }
                                    else if (File.Exists(trffileTgtPath))
                                    {
                                        Console.WriteLine("file " + trffileTgtPath + " already copied");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Copying " + trffileSrcPath + " to " + trffileTgtPath);
                                        File.Copy(trffileSrcPath, trffileTgtPath);
                                    }
                                }

                                FilesDate = FilesDate.AddDays(1);
                            }

                            UpdateDate(FilesDate);

                            context.Undo();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine("[" + DateTime.Now.ToShortTimeString() +  "] Suspending");
                Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["interval"]));
            }
        }

        static void UpdateDate(DateTime d)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["Date"].Value = d.ToString("yyyy-MM-dd");
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    
}
