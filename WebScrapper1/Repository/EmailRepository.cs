using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.Models;
using WebScrapper.Interfaces;

namespace WebScrapper.Repository
{
    public class EmailRepository(): IEmailRepository
    {

        public void SendEmail(SmtpSettings smtpSettings, EmailMessage emailMessage)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(smtpSettings.FromEmail);
                mail.To.Add(smtpSettings.emailToAddress);
                mail.Subject = emailMessage.subject;
                mail.Body = emailMessage.body;
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(smtpSettings.FromEmail, (smtpSettings.Password));
                    smtp.EnableSsl = smtpSettings.EnableSSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}
