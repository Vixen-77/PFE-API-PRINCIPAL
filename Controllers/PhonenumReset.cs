using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using LibrarySSMS;
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using APIAPP.Services;
using APIAPP.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using APIAPP.Exceptions;
using System.Data;

[Route("api/changenumber")]
[ApiController]
public class PhoneNumbertoEmail : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly UploaderPatient _methodePatientService;
    private readonly AppDbContext _context;
    private readonly GlobalService _globalService;

    public PhoneNumbertoEmail(
        EmailService emailService,
        AppDbContext context,
        UploaderPatient methodePatientService,
        GlobalService globalService)
    {
        _emailService = emailService;
        _context = context;
        _methodePatientService = methodePatientService;
        _globalService = globalService;
    }

            
   [HttpPost("phonenumber")]
   [EnableCors("AllowReactApp")]
   public async Task<IActionResult> RequestCode([FromForm] string Role, [FromForm] string email)
   {
        switch (Role)
       {
        case "10": // Patient
             
             var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
                if (patient == null)
                {
                    throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
                }

                var emailptosend = patient.Email;
                var code1 = _globalService.GenerateCode(emailptosend);

               string head ="Code de réinitialisation de numero de telephone";
               string body = $"<h3>Code de chanhement de numero de telephone </h3><p>Votre code est :</p>";

             var good = await _emailService.SendCodeEmail(emailptosend, head, body, code1);
             if (!good)
                {
                     return BadRequest("Erreur lors de l'envoi de l'email.");
                }
   
             return Ok("Un code a été envoyé à votre email");
 
        case "20": // Professionnel de santé
           
             var pros = await _context.ProSs.FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
                if (pros == null)
                {
                    throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
                }

                var emailptosendpro = pros.Email;
                var code2 = _globalService.GenerateCode(emailptosendpro);

               string head2 ="Code de réinitialisation de numero de telephone";
               string body2 = $"<h1>Code de chanhement de numero de telephone </h1><p>Votre code est :</p>";

             var good2 = await _emailService.SendCodeEmail(emailptosendpro, head2, body2, code2);
             if (!good2)
                {
                     return BadRequest("Erreur lors de l'envoi de l'email.");
                }
   
             return Ok("Un code a été envoyé à votre email");

        default:
            return BadRequest("Rôle non reconnu.");
       }

    }

    [HttpPost("submit-code")]
    [EnableCors("AllowReactApp")]
    public IActionResult SubmitCode([FromForm] CodeSubmissionDto submission)
    {
      bool valid = _globalService.ValidateCode(submission.Email, submission.Code);
     return valid 
    ? Ok("Code correct") 
    : BadRequest("Code invalide");
    }


     [HttpPost("newphonenumber")]
     [EnableCors("AllowReactApp")]
     public async Task<IActionResult> NewPhoneNum([FromForm] string Uid,[FromForm] string role,[FromForm] string newPhoneNumber)
    {
                var UserID=Guid.Parse(Uid);

                if (role == "10")
                {
                    var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == UserID);
                    if (patient != null)
                    {  
                         patient.PhoneNumber = newPhoneNumber;   
                        await _context.SaveChangesAsync();
                        return Ok("numero de tel modifié avec succès.");
                    }

                }
                else if (role == "20")
                {
                    var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == UserID);
                    if (proS != null)
                    {
                        proS.PhoneNumber = newPhoneNumber;
                        await _context.SaveChangesAsync();
                        return Ok("Mot de passe modifié avec succès.");
                    }
                }
            
        return BadRequest("Une erreur s'est produite lors de la soumission du code.");
    }
}