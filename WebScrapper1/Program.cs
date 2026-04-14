using System;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using HtmlAgilityPack;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using WebScrapper.Models;
using WebScrapper.Repository;
using WebScrapper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text;


namespace WebScrapper
{
    internal class Program
    {

        static void Main(string[] args)
        {
            IEmailRepository emailRepo = new EmailRepository();


            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var smtpSettings = config
                .GetSection("SmtpSettings")
                .Get<SmtpSettings>();

            

            var message = emailRepo.CreateMail("https://www.basketball-reference.com/boxscores/?month=4&day=12&year=2026");
            

            var emailMessage = new EmailMessage
            {
                subject = $"🏀 NBA Daily - {DateTime.Now:dd MMMM yyyy}",
                body = message.ToString()
            };


            emailRepo.SendEmail(smtpSettings, emailMessage);


        }


    }
}
