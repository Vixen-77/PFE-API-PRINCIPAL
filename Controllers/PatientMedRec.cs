using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using LibrarySSMS;
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using APIAPP.Services;

[Route("api/mail")]
[ApiController]
public class Controlleurtest2 : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly UploaderPatient _methodePatientService;
    private readonly AppDbContext _context;

    public Controlleurtest2(
        EmailService emailService,
        AppDbContext context,
        UploaderPatient methodePatientService)
    {
        _emailService = emailService;
        _context = context;
        _methodePatientService = methodePatientService;
    }

    [HttpPost("PatientUploder")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> UploadPatient([FromForm] IFormFile file, Guid patientid, string MailduMedcin)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Fichier non fourni ou vide.");
          Guid patientGlobal=patientid;
        // Appel au service pour uploader le fichier et envoyer l'email
        string thepathishere = await _methodePatientService.UploadandEmail(file, patientid, MailduMedcin);

        if (string.IsNullOrEmpty(thepathishere))
            return BadRequest(500);
        else {
            
        {
          string subject = "Nouveau dossier médical reçu";
          string body = $"Bonjour,\n\nVous avez reçu un nouveau dossier médical concernant un patient.\n\nVeuillez vérifier si vous êtes bien à l'origine de cette prescription.\n\nMerci de votre vigilance.";

            var isSent = await _emailService.SendEmailAsync(MailduMedcin, subject, body, thepathishere);
            if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email.");
        }
        }

       var response = new
        {
            Message = "votre Docier Medical a était envoyé avec succes a votre medecin",
            Timestamp = DateTime.UtcNow,
        };
        
        return Ok(response);
    }

   [HttpPost("PatientReccup")]
   [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ValidateMedRec(int validation)
    {
        var medRec = await _context.MedRecs.FindAsync(id);
        if (medRec == null) return NotFound("Dossier introuvable");

        medRec.State = MedRecState.Pawding;
        await _context.SaveChangesAsync();

        return Content("✅ Dossier validé avec succès !");
    }

    [HttpGet("refuse")]
    public async Task<IActionResult> RefuseMedRec(Guid id)
    {
        var medRec = await _context.MedRecs.FindAsync(id);
        if (medRec == null) return NotFound("Dossier introuvable");

        medRec.State = MedRecState.Refused;
        await _context.SaveChangesAsync();

        return Content("❌ Dossier refusé.");
    }
}







}

