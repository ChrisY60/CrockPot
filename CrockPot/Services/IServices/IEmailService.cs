namespace CrockPot.Services.IServices
{
    public interface IEmailService
    {
        Task <bool> SendEmailAsync (string TargetAddress, string Subject, string HtmlContent);
    }
}
