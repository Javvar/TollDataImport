using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.NLogger;
using Microsoft.Exchange.WebServices.Data;


namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = ConfigurationManager.AppSettings["StoredProcBCCPassword"];

            string Username = "bankinterface";
            string Password = "B@nk!nt3rf@c3";
            string MailService = "https://mail.g5.co.za/EWS/Exchange.asmx";

            var service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            service.Credentials = new WebCredentials(Username, Password);
            service.Url = new Uri(MailService);

            FindFoldersResults Inbox = service.FindFolders(WellKnownFolderName.Inbox, new FolderView(100));
            FindFoldersResults SiteFolder = service.FindFolders(Inbox.First(x => x.DisplayName == "N4W").Id, new FolderView(10));

             Inbox = service.FindFolders(WellKnownFolderName.Inbox, new FolderView(100));
             SiteFolder = service.FindFolders(Inbox.First(x => x.DisplayName == "N4W").Id, new FolderView(10));
            FindFoldersResults BankFolders = service.FindFolders(SiteFolder.First(x => x.DisplayName == "Banks").Id, new FolderView(10));


            string Banks = @"N4W_Standard Bank|DirectoryPath_StandardBankFleet|.txt; 
								N4W_WesBank|DirectoryPath_FirstAuto|.zip; 
								N4W_Nedbank|DirectoryPath_NedFleet|.xls; 
								N4W_ABSAFleet|DirectoryPath_AbsaFleet|.xls";
            List<string> BankInfo = Banks.Replace("\r\n", "").Split(';').ToList();

            foreach (string bnk in BankInfo)
            {
                var BankFolderName = bnk.Split('|')[0].TrimStart();
                var BankDropFolderSetting = bnk.Split('|')[1].TrimStart();
                var BankFileExtension = bnk.Split('|')[2].TrimStart();
                //var BankDropFolder = ConfigurationManager.AppSettings[BankDropFolderSetting];

                try
                {
                    Log.LogInfoMessage(string.Format("Checking new emails for bank {0}", BankFolderName));

                    var BankFolder = BankFolders.Folders.First(x => x.DisplayName == BankFolderName.ToString()).Id;

                    var searchFilterOperator = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);
                    var filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, searchFilterOperator);

                    var findResults = service.FindItems(BankFolder, filter, new ItemView(10));

                    if (findResults != null && findResults.Items != null && findResults.Items.Count > 0)
                    {
                        foreach (Item item in findResults.Items)
                        {
                            var PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments,
                                ItemSchema.HasAttachments);
                            var message = EmailMessage.Bind(service, item.Id, PropertySet);

                            foreach (Attachment attachment in message.Attachments)
                            {
                                try
                                {
                                    if (attachment is FileAttachment)
                                    {
                                        FileAttachment FileAttachment = attachment as FileAttachment;

                                        if (BankFileExtension.Contains(Path.GetExtension(FileAttachment.Name)))
                                        {
                                            FileAttachment.Load();
                                            Log.LogTrace(string.Format("Loaded file {0}", FileAttachment.Name));
                                        }
                                        else
                                        {
                                            continue;
                                        }


                                        Log.LogInfoMessage(string.Format("Found file {0} in {1}", FileAttachment.Name,
                                            BankFolderName));

                                        //var TempFilename =
                                        //    Path.GetFileNameWithoutExtension(FileAttachment.Name).Length > 50
                                        //        ? Path.GetFileNameWithoutExtension(FileAttachment.Name).Substring(0, 50)
                                        //        : Path.GetFileNameWithoutExtension(FileAttachment.Name) + ".tmp";
                                        ////File.WriteAllBytes(Path.Combine(BankDropFolder, TempFilename),
                                        ////    FileAttachment.Content);

                                        //var Filename = Path.ChangeExtension(TempFilename,
                                        //    Path.GetExtension(FileAttachment.Name));
                                        //File.Move(Path.Combine(BankDropFolder, TempFilename),
                                        //    Path.Combine(BankDropFolder, Filename));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Log.LogException(ex, "Could not download attachment");
                                    //SendEmail(string.Format("Email attachment download: {0}", BankFolderName),
                                    //    string.Format("Could not download attachment from {0}. {1}", BankFolderName,
                                    //        ex.ToString()));
                                }
                            }

                            var mes = (EmailMessage) item;
                            mes.IsRead = true;
                            mes.Update(ConflictResolutionMode.AutoResolve);
                        }
                    }
                    {
                        Log.LogInfoMessage(string.Format("No new files found for bank {0}", BankFolderName));
                    }
                }
                catch (Exception ex)
                {
                    Log.LogException(ex,
                        string.Format("while checking for new hotlist files for bank {0}", BankFolderName));
                }
            }
        }
    }
}
