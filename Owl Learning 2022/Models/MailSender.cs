using Owl_Learning_2022.Models;
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

        owldbEntities owldb = new owldbEntities();
        public bool SendEmail(string emailTo, string name, string subject, string body)
        {
            var key = "";
            key = owldb.questions.Where(a => a.question_id == 1).FirstOrDefault().question;

            try
            {
                var client = new SendGridClient(key);
                var from = new EmailAddress("owllearningcr@gmail.com", "Owl Learning");
                var to = new EmailAddress(emailTo.Trim(), name);
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