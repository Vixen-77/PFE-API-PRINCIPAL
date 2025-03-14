using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Models;
using APIAPP.Services;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIAPP.Data;

[Route("api/uploadsPatient")]
[ApiController]
public class UploaderPatient : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IEmailService _emailService; 
    private readonly ILogger<UploaderPatient> _logger;
    private readonly ApplicationDbContext _context;  
    private readonly UploadindatabaseService _uploaddatabase;

    public UploaderPatient(IEmailService emailService,UploadindatabaseService uploadindatabase,AuthService authService ,ApplicationDbContext context, ILogger<UploaderPatient> logger)
    {   
        _uploaddatabase =uploadindatabase;
        _authService = authService;
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }

    [HttpPost("signup-and-upload")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> SignupAndUpload(IFormFile file, [FromForm] Guid userId)

    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var uploadFolder = Path.Combine("C:\\Users\\ASUS\\Desktop\\PFE3.0\\APIprincipal\\APIAPP\\Datapatient");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var filePath = Path.Combine(uploadFolder, file.FileName);
        await _uploaddatabase.UpdatePatientFilePath(userId, filePath);

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //TODO: Récupération du patient et de l'email du médecin depuis la base de données
            var patient = await _context.Patients.FindAsync(userId);
            

            if (patient == null)
            {
                return NotFound("Patient non trouvé.");
            }

            string doctorEmail = patient.MailMed;
            string  notrepatientmail = patient.Email;
            if (string.IsNullOrEmpty(doctorEmail))
            {
                return BadRequest("L'email du médecin n'est pas défini.");
            }

            // 📧 Génération du contenu de l'e-mail
            string validationUrl = $"http://localhost:5000/api/uploadsPatient/validate?userId={userId}&approve=true";
            string rejectionUrl = $"http://localhost:5000/api/uploadsPatient/validate?userId={userId}&approve=false";

            string emailBody = $@"
                <h2>Validation du Dossier Patient</h2>
                <p>Un nouveau patient s'est inscrit. Vous pouvez consulter son dossier et le valider.</p>
                <p><a href='{validationUrl}'>✅ Confirmer</a> | <a href='{rejectionUrl}'>❌ Refuser</a></p>
            ";

            // ✉️ Envoi de l'email au médecin
            bool emailSent = await _emailService.SendEmailAsync(doctorEmail, "Validation du Dossier Patient", emailBody);
            
            if (!emailSent)
            {
                throw new Exception("Échec de l'envoi de l'e-mail au médecin.");
            }
            else{
                 bool changementvalidationtotrue = _authService.Confirmvalidation(notrepatientmail);
                 bool emailSentauPatient = await _emailService.SendEmailAsync(notrepatientmail, "Validation du Dossier Patient", emailBody);  
             
                 return Ok(new { message = "Patient enregistré, e-mail envoyé au médecin pour validation." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription et du téléchargement du fichier.");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}