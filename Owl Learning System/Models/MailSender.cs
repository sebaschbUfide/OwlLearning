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
        public void SendMail(String to, String subject, String body)
        {
            var sender = new SmtpSender(() => new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 587,
                Credentials = new NetworkCredential("owllearningcr@gmail.com", "owllearning2021"),
                EnableSsl = true
            });

            Email.DefaultSender = sender;

            var email = Email
                .From("owllearningcr@gmail.com")
                .To(to)
                .Subject(subject)
                .Body(body);

            try
            {
                email.SendAsync();
            }
            catch (Exception e)
            {

            }
            
        }

        public async Task sendEmailUsingSendGrid(string emailTo, string name, string subject, string body)
        {
            var client = new SendGridClient("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");
            ////send an email message using the SendGrid Web API with a console application.  
            var msgs = new SendGridMessage()
            {
                From = new EmailAddress("owllearningcr@gmail.com", "Owl Learning"),
                Subject = subject,
                TemplateId = "fb09a5fb-8bc3-4183-b648-dc6d48axxxxx",
                ////If you have html ready and dont want to use Template's   
                //PlainTextContent = "Hello, Email!",  
                //HtmlContent = "<strong>Hello, Email!</strong>",  
            };
            //if you have multiple reciepents to send mail  
            msgs.AddTo(new EmailAddress(emailTo, name));
            //If you have attachment  
            var attach = new SendGrid.Helpers.Mail.Attachment();
            //attach.Content = Convert.ToBase64String("rawValues");  
            attach.Type = "image/png";
            attach.Filename = "hulk.png";
            attach.Disposition = "inline";
            attach.ContentId = "hulk2";
            //msgs.AddAttachment(attach.Filename, attach.Content, attach.Type, attach.Disposition, attach.ContentId);  
            //Set footer as per your requirement  
            msgs.SetFooterSetting(true, "<strong>Regards,</strong><b> Pankaj Sapkal", "Pankaj");
            //Tracking (Appends an invisible image to HTML emails to track emails that have been opened)  
            //msgs.SetClickTracking(true, true);  
            var responses = await client.SendEmailAsync(msgs);
        }

        public static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("owllearningcr@gmail.com", "Owl Learning");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("frr9724@gmail.com", "Francisco");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);
            var result4 = Task.Run(() => client.SendEmailAsync(msg)).GetAwaiter().GetResult();
        }

        public bool SendEmail(string emailTo, string name, string subject, string body)
        {
            try
            {
                var client = new SendGridClient("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");
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