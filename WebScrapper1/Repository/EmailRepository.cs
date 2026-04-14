using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.Models;
using WebScrapper.Interfaces;
using HtmlAgilityPack;

namespace WebScrapper.Repository
{
    public class EmailRepository() : IEmailRepository
    {
        public StringBuilder CreateMail(string url)
        {
            StringBuilder message = new StringBuilder();
            Dictionary<string, string> teamColors = new Dictionary<string, string>
{
    {"Atlanta", "#E03A3E"},
    {"Boston", "#007A33"},
    {"Brooklyn", "#000000"},
    {"Charlotte", "#1D1160"},
    {"Chicago", "#CE1141"},
    {"Cleveland", "#6F263D"},
    {"Dallas", "#00538C"},
    {"Denver", "#0E2240"},
    {"Detroit", "#C8102E"},
    {"Golden State", "#1D428A"},
    {"Houston", "#CE1141"},
    {"Indiana", "#002D62"},
    {"LA Clippers", "#C8102E"},
    {"Los Angeles", "#552583"},
    {"Memphis", "#5D76A9"},
    {"Miami", "#98002E"},
    {"Milwaukee", "#00471B"},
    {"Minnesota", "#0C2340"},
    {"New Orleans", "#0C2340"},
    {"New York", "#006BB6"},
    {"Oklahoma City", "#007AC1"},
    {"Orlando", "#0077C0"},
    {"Philadelphia", "#006BB6"},
    {"Phoenix", "#1D1160"},
    {"Portland", "#E03A3E"},
    {"Sacramento", "#5A2D81"},
    {"San Antonio", "#C4CED4"},
    {"Toronto", "#CE1141"},
    {"Utah", "#002B5C"},
    {"Washington", "#002B5C"}
};

            string GetColor(string team)
            {
                foreach (var key in teamColors.Keys)
                {
                    if (!string.IsNullOrEmpty(team) && team.Contains(key))
                        return teamColors[key];
                }
                return "#ffffff";
            }
            // =======================
            // HTML + STYLING
            // =======================
            message.AppendLine(@"
<html>
<head>
<style>
    body {
        margin: 0;
        padding: 0;
        background-color: #0b0b0f;
        font-family: Arial;
        color: white;
    }

    .container {
        max-width: 720px;
        margin: auto;
        padding: 20px;
    }

    .header {
        text-align: center;
        font-size: 30px;
        font-weight: 900;
        color: #ff2d2d;
        margin-bottom: 25px;
    }

    .subheader {
        text-align: center;
        color: #9ca3af;
        margin-bottom: 25px;
    }

    .game-card {
        background: #15151c;
        border-left: 5px solid #ff2d2d;
        border-radius: 10px;
        padding: 15px;
        margin-bottom: 15px;
    }

    .teams {
        font-size: 18px;
        font-weight: bold;
        margin-bottom: 8px;
    }

    .vs {
        color: #ffffff;
        font-weight: bold;
        margin: 0 8px;
    }

    .score {
        font-size: 26px;
        font-weight: 900;
        color: #fbbf24;
        margin-bottom: 6px;
    }

    .status {
        font-size: 13px;
        color: #9ca3af;
        margin-bottom: 10px;
    }

    a {
        color: #38bdf8;
        font-weight: bold;
        text-decoration: none;
    }

    .footer {
        text-align: center;
        font-size: 12px;
        color: #6b7280;
        margin-top: 20px;
    }
</style>
</head>
<body>
<div class='container'>
");

            // =======================
            // HEADER
            // =======================
            message.AppendLine($@"
<div class='header'>ESPN NBA DAILY</div>
<div class='subheader'>Games for {DateTime.Now:dd MMMM yyyy}</div>
");

            // =======================
            // LOAD PAGE
            // =======================
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            var games = doc.DocumentNode.SelectNodes("//div[contains(@class,'game_summary')]");

            if (games != null)
            {
                foreach (var game in games)
                {
                    var teamRows = game.SelectNodes(".//table[@class='teams']//tr");

                    string team1 = "", team2 = "";
                    string score1 = "", score2 = "";
                    string status = "";
                    string boxLink = "";

                    if (teamRows != null && teamRows.Count >= 2)
                    {
                        var row1 = teamRows[0];
                        var row2 = teamRows[1];

                        team1 = row1.SelectSingleNode(".//td[1]/a")?.InnerText.Trim();
                        score1 = row1.SelectSingleNode(".//td[@class='right']")?.InnerText.Trim();

                        team2 = row2.SelectSingleNode(".//td[1]/a")?.InnerText.Trim();
                        score2 = row2.SelectSingleNode(".//td[@class='right']")?.InnerText.Trim();
                    }

                    status = game.SelectSingleNode(".//td[contains(@class,'gamelink')]//a")
                                 ?.InnerText.Trim();

                    var linkNode = game.SelectSingleNode(".//p[@class='links']//a[contains(@href,'boxscores')]");
                    if (linkNode != null)
                        boxLink = "https://www.basketball-reference.com" +
                                  linkNode.GetAttributeValue("href", "");

                    string team1Color = GetColor(team1);
                    string team2Color = GetColor(team2);

                    message.AppendLine($@"
        <div class='game-card'>

            <div class='teams'>
                <span style='color:{team1Color}'>{team1}</span>

                <span class='vs'>vs</span>

                <span style='color:{team2Color}'>{team2}</span>
            </div>

            <div class='score'>
                {score1} - {score2}
            </div>

            <div class='status'>{status}</div>

            <div>
                <a href='{boxLink}'>📊 Full Box Score</a>
            </div>

        </div>
        ");
                }
            }

            // =======================
            // FOOTER
            // =======================
            message.AppendLine(@"
<div class='footer'>
    Powered by NBA Scraper • ESPN Style Email
</div>

</div>
</body>
</html>
");

            return message;
        }

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
