using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{     

               //TODO: role de cette methode en utilsant le smtp mais bon faut verifier avec damya ce soir 
    /*methode denvoi au medecin mei dans le formulaire  et elle sera aussi utiliser pour l'envoi du
    mail du patient apres validation pour lui informé qu'il peut se connecter a son compte*/
   public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
{   
    try
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("tonemail@gmail.com", "motdepasse"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("tonemail@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
        return true;
    }
    catch (Exception)
    {
        return false;
    }
}
}
