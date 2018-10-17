using System;
using System.Linq;
using Intertoll.Toll.DataImport.Interfaces;

namespace Intertoll.DataImport.Mail
{
    public class NewStaffMailFormatter : INewStaffMailFormatter
    {
        private string Template = @"<br/><table><tr><td>{0}</td></tr></table>";

        public string Format(string stringToFormat, char delimiter)
        {
            stringToFormat = stringToFormat.Insert(0, "The following collectors were automatically created. The user details need to be captured manually via the PCS <br/><br/>");
            return stringToFormat.Split(delimiter).Aggregate((x, y) => x + String.Format(Template, y));
        }
    }

    public class DuplicateTransactionMailFormatter : IDuplicateTransactionMailFormatter
    {
        private string Template = @"<br/><table><tr><td>{0}</td></tr></table>";

        public string Format(string stringToFormat, char delimiter)
        {
            return stringToFormat.Split(delimiter).Aggregate((x, y) => x + String.Format(Template, y));
        }
    }
}
