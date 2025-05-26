using System.Net.Mail;
using server.Contracts;
using server.Data;
using server.Models.SMTPModels;
using static server.Models.ProjectEnum;

namespace  server.Services;

public class SMTPService : ISMTPService
{

    private readonly TheDailyLensContext context;

    private readonly IConfiguration configuration;


    public SMTPService(TheDailyLensContext _context, IConfiguration _configuration)
    {
        context = _context;
        configuration = _configuration;
    }



    public async Task<bool> IsSubscribed(string email)
    {
        bool isSub = context.Subscribes.Any(x => x.Email == email);
        return isSub;
    }


    public async Task<bool> updateSubscriptionStatus(updateSubscriptionStatusModel model)
    {

        if (model.isSubscribed == true)
        {
            context.Subscribes.Add(new() { Email = model.Email });
            await context.SaveChangesAsync();
        }
        else
        {
            Subscribe subscribe = context.Subscribes.FirstOrDefault(x => x.Email == model.Email);
            context.Subscribes.Remove(subscribe);
            await context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> SendEmail(sendEmail data)
    {
        try
        {
            string smtpSettings = SmtpSettings.SmtpSettings.ToString();
            string host = configuration[$"{smtpSettings}:{SmtpSettings.Host}"];
            _ = int.TryParse(configuration[$"{smtpSettings}:{SmtpSettings.Port}"], out int port);
            string username = configuration[$"{smtpSettings}:{SmtpSettings.Username}"];
            string password = configuration[$"{smtpSettings}:{SmtpSettings.Password}"];

              port = port is > 0 and < 65536 ? port : 587;

            using var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "The Daily Lens"),
                Subject = data.subject,
                Body = data.body,
                IsBodyHtml = true
            };

            List<string> subscribedEmails = context.Subscribes.Select(x => x.Email).ToList();

            foreach (string email in subscribedEmails)
            {
                if (email == data.currentEmail)
                {
                    continue;
                }

                mailMessage.Bcc.Add(new MailAddress(email));
            }

            if (mailMessage.Bcc.Count == 0)
            {
                return false;
            }

            await smtpClient.SendMailAsync(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }


}