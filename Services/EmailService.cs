using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService(IConfiguration configuration)
    {
        _smtpServer = configuration["Email:SmtpServer"] ?? throw new ArgumentNullException("Email:SmtpServer configuration is missing");
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? throw new ArgumentNullException("Email:SmtpPort configuration is missing"));
        _smtpUser = configuration["Email:Username"]  ?? throw new ArgumentNullException("non d'utilisateur invalide");
        _smtpPass = configuration["Email:Password"] ?? throw new ArgumentNullException("Email:Password configuration is missing");
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string filePath)
    {
        try
        {
            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                var attachment = new Attachment(filePath);
                mailMessage.Attachments.Add(attachment);
            }

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
            return false;
        }
    }
}
