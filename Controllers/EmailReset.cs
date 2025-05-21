using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Twilio;
using System.Threading.Tasks;
using APIAPP.Services;
using APIAPP.DTO;
using APIAPP.Exceptions;
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;



    [Route("api/changemail")]
    [ApiController]
    public class EmailtoSms : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly AppDbContext _context;
        private readonly GlobalService _globalService;
        private readonly EmailService _emailService;
        private readonly object? _monip;

    public EmailtoSms(ISmsService smsService, AppDbContext context, GlobalService globalService, EmailService emailService, IConfiguration configuration)
    {
        _smsService = smsService;
        _context = context;
        _globalService = globalService;
        _emailService = emailService;
        _monip = configuration["ipadr"] ?? throw new ArgumentException("ip manquant dans la configuration.");

    }
    
        [HttpPost("email")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> SendSms([FromForm] string role , [FromForm] string id)
        {
            if (role == "10"){


                var idp = Guid.Parse(id);
                var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == idp);
                if (patient == null)
                {
                    throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
                }
                string PhoneNumberP = "+213556319616";

                // Vérifie que le numéro commence par "0" et a une longueur suffisante
               /* if (!string.IsNullOrEmpty(PhoneNumberP) && PhoneNumberP.StartsWith("0"))
                {
                         // Enlève le 0 et ajoute l'indicatif +213
                         PhoneNumberP = "+213" + PhoneNumberP.Substring(1);
                         Console.WriteLine(PhoneNumberP);
                }
                */

                var code1 = _globalService.GenerateCodePhoneNumber(PhoneNumberP);
                string message = "Votre code de réinitialisation de votre Email est : " + code1;

                 var (sid, status) = await _smsService.SendSmsAsync(PhoneNumberP, message);
                 return Ok(new { MessageSid = sid, Status = status });
                 }


            else if (role == "20")
            {
                var idp = Guid.Parse(id);
                var pros = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == idp);
                if (pros == null)
                {
                    throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
                }
                string PhoneNumberP = "+213556319616";

                // Vérifie que le numéro commence par "0" et a une longueur suffisante
                /* if (!string.IsNullOrEmpty(PhoneNumberP) && PhoneNumberP.StartsWith("0"))
                {
                         // Enlève le 0 et ajoute l'indicatif +213
                         PhoneNumberP = "+213" + PhoneNumberP.Substring(1);
                         Console.WriteLine(PhoneNumberP);
                }
                */

                var code1 = _globalService.GenerateCodePhoneNumber(PhoneNumberP);
                string message = "Votre code de réinitialisation de votre Email est : " + code1;

                 var (sid, status) = await _smsService.SendSmsAsync(PhoneNumberP, message);
                 return Ok(new { MessageSid = sid, Status = status });
            }
            else
            {
                return BadRequest("Rôle non reconnu.");
            }   
        }

        [HttpPost("submit-code")]
        [EnableCors("AllowReactApp")]
        public IActionResult SubmitCode([FromForm] string phonenumber,[FromForm] string code)
        {
        bool isValid = _globalService.ValidateCodePhoneNumber(phonenumber, code);
        if (!isValid)
        {
        return BadRequest("Code invalide ou expiré.");
        }
        return Ok("Code valide.");
        }


        [HttpPost("validate-sms")]
        [EnableCors("AllowReactApp")] 
        public async Task<IActionResult> ValidateSmsAndChangeEmail([FromForm] string newEmail,  [FromForm]string role, [FromForm] string id)
    {   

    var uid = Guid.Parse(id);

    if (role == "10") // patient
    {
        var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
        if (patient == null)
            return NotFound("Patient introuvable.");
        var existingPatient = await _context.Patientss.FirstOrDefaultAsync(p => p.Email.ToLower() == newEmail.ToLower());

        if (existingPatient != null)
            return Conflict("This Email adress is already taken.");

        patient.Email = newEmail;
        await _context.SaveChangesAsync();
    }
    else if (role == "20") // pro
    {
        var pro = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
        if (pro == null)
            return NotFound("Professionnel introuvable.");

        var existingPro = await _context.ProSs.FirstOrDefaultAsync(p => p.Email.ToLower() == newEmail.ToLower());
        if (existingPro != null)
            return Conflict("This Email adress is already taken.");

        pro.Email = newEmail;
        await _context.SaveChangesAsync();
    }
    else
    {
        return BadRequest("Rôle invalide.");
    }

    // Envoi d'un email de confirmation
          string baseUrl = $"http://{_monip}:5001";  
          string subject = "Please Confirm Your New Email Address";
          string body = $@"
    <html>
      <body>
        <p>Bonjour,</p>
        <p>We received a request to update the email address associated with your account on E-Mergency.</p>
        <p>To confirm that this new email belongs to you and complete the update, please click the link below:</p>
        <br>
        <a href={baseUrl}/api/changemail/validate?id={id}&role={role}>Confirm New Mail</a>
        <br><br>
        <p>Thanks for being part of E-Mergency!</p>
        <p>Best regards,</p>
        <p>The E-Mergency Team</p>
      </body>
    </html>";
    var isSent= await _emailService.SendEmailAsyncValidation(newEmail, subject, body);
    if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email de confirmation.");
        
    return Ok("Email mis à jour avec succès.");
    }

    [HttpGet("validate")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ValidateEmail(string id, string role)
    {
        if(role=="10"){
        var uid = Guid.Parse(id);
        var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
        if (patient == null)
            return NotFound("Patient introuvable.");

        patient.ConfMail=true;
        await _context.SaveChangesAsync();
        }
        else if(role=="20"){
        var uid = Guid.Parse(id);
        var pro = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
        if (pro == null)
            return NotFound("Professionnel introuvable.");
        pro.ConfMail=true;
        await _context.SaveChangesAsync();
        }
        else
        {
            return BadRequest("Rôle invalide.");
        }

        return Ok("Email validé avec succès.");

    }
    }