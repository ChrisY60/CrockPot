

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
