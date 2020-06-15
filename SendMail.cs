using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace HttpPostSendMail
{
    public static class SendMail
    {
        [FunctionName("SendMail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("SendMail based on form data");

            var stringBuilder = new StringBuilder();
            var htmlBuilder = new StringBuilder();

            foreach (var key in req.Form.Keys) {
                stringBuilder.AppendLine($"{key}:{req.Form[key].ToString()}"); 
                htmlBuilder.AppendLine($"<p> {key}:{req.Form[key].ToString()} </p>");
            }

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("igormusic@tvmsoftware.com", "Igor Music");
            var subject = "Test SendGrid";
            var to = new EmailAddress("jelenabozic@hotmail.com", "Jelena Bozic");
            var plainTextContent = stringBuilder.ToString();
            var htmlContent = htmlBuilder.ToString();
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return new RedirectResult("https://www.google.com");
        }
    }
}
