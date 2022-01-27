using FluentEmail.Core;
using FluentEmail.Smtp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace mysqltest.Models
{
    public class MailSender
    {
   
        public bool SendEmail(string emailTo, string name, string subject, string body)
        {
            var key = "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";

            try
            {
                var client = new SendGridClient(key);
                var from = new EmailAddress("owllearningcr@gmail.com", "Owl Learning");
                var to = new EmailAddress(emailTo.Trim(), name);
                //var htmlContent = AlternateView.CreateAlternateViewFromString(plainTextContent, null, MediaTypeNames.Text.Html);
                //var htmlContent = "<strong>esto es una prueba</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "PLAIN TEXT", body);
                var response = client.SendEmailAsync(msg);

                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
                throw;
            }
        }
        
    }
}