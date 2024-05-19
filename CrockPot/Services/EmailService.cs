using CrockPot.Services.IServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace CrockPot.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string TargetAddress, string Subject, string HtmlContent)
        {
            var keyVaultUrl = _configuration["KeyVault:KeyVaultUrl"];
            var keyVaultClientId = _configuration["KeyVault:ClientId"];
            var keyVaultClientSecret = _configuration["KeyVault:ClientSecret"];
            var keyVaultDirectoryId = _configuration["KeyVault:DirectoryId"];

            var credential = new ClientSecretCredential(keyVaultDirectoryId, keyVaultClientId, keyVaultClientSecret);
            var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

            var apiKey = (await secretClient.GetSecretAsync("APIKeySendGrid1")).Value.Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("crockpot0206@gmail.com", "CrockPot");
            var subject = Subject;
            var to = new EmailAddress(TargetAddress, "CrockPot User");
            var plainTextContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, HtmlContent);
            var response = await client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }
    }
}
