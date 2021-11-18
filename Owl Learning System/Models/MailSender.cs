using FluentEmail.Core;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

    }
}