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

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://example.com/");

            var Title = doc.DocumentNode.SelectNodes("//div/h1").First().InnerText;

            var Body = doc.DocumentNode.SelectNodes("//div/p").First().InnerText;

            Console.WriteLine($"{Title} \n");
            Console.WriteLine($"{Body} \n");

            var emailMessage = new EmailMessage
            {
                subject = "Hello",
                body = Body
            };

            emailRepo.SendEmail(smtpSettings, emailMessage);


        }


    }
}
