

using CrockPot.Services.IServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace CrockPot.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(string TargetAddress, string Subject, string HtmlContent)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("crockpot0206@gmail.com", "CrockPot");
            var subject = Subject;
            var to = new EmailAddress(TargetAddress, "CrockPot User");
            var plainTextContent = "";
            var htmlContent = HtmlContent;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return true;
            //dobavi normalen return
        }

    }
}
