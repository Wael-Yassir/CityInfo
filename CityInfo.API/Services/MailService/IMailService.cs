namespace CityInfo.API.Services.MailService
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}