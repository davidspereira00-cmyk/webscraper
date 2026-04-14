using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.Models;

namespace WebScrapper.Interfaces
{
    internal interface IEmailRepository
    {
        void SendEmail(SmtpSettings smtpSettings, EmailMessage emailMessage);
    }
}
