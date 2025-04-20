using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("api/testmail")]
[ApiController]
public class Controlleurtest : ControllerBase
{   
    [HttpPost("Emailsendertest")]
    public async Task<IActionResult> TestGmail()
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("licpfe2025@gmail.com", "biqppsbqyhjhsmvw"), // Remplace ici
            EnableSsl = true
        };



        string emailBody = $@"
          
         MARIA UR SUPER PRETTY


        — Message automatically sent by API-PFE-Principale🚀
         ";


        var mailMessage = new MailMessage
        {
            From = new MailAddress("licpfe2025@gmail.com"),
            Subject = "FOR MARIA",
            Body = emailBody,
            IsBodyHtml = false
        };
           var path ="C:\\Users\\ASUS\\Desktop\\PFE3.0\\APIprincipal\\APIAPP\\Datapatient\\cat.jpg";
           mailMessage.To.Add("raniaderriche02@gmail.com"); //mettre le mail de tu sait qui lol
            var attachment = new Attachment(path);
            mailMessage.Attachments.Add(attachment);
     
        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            return Ok("Email envoyé avec succès !");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erreur lors de l'envoi de l'email : {ex.Message}");
        }
    }
}