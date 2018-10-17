namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface IMailClient
    {
        void SendMessage<T>(string subject, string message) where T : IMailFormatter;
    }

    public interface IMailFormatter
    {
        string Format(string stringToFormat, char delimiter);
    }

    public interface INewStaffMailFormatter : IMailFormatter
    {
    }

    public interface IDuplicateTransactionMailFormatter : IMailFormatter
    {
    }
}
