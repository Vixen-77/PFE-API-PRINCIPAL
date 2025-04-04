using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibrarySSMS;

[Route("api/uploadsPatient")]
[ApiController]
public class UploaderPatient : ControllerBase
{
    private readonly AppDbContext _dbContext;         
    private readonly IEmailService _emailService;
    private readonly UploadindatabaseService _uploaddatabase;
    private readonly AuthService _authService;
    private readonly ILogger<UploaderPatient> _logger;

    public UploaderPatient( IEmailService emailService,UploadindatabaseService uploadindatabase,AuthService authService,AppDbContext dbContext,ILogger<UploaderPatient> logger)
    {
        _uploaddatabase = uploadindatabase;
        _authService = authService;
        _emailService = emailService;
        _dbContext = dbContext; // Correction ici
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
            var patient = await   _dbContext.Patientss.FindAsync(userId);
            

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





            // TODO: définir le corp des mail envoyé au medecin 
            string emailBody = $@"
             <h2>Validation du Dossier Patient</h2>
             <p>Un nouveau patient s'est inscrit. Vous pouvez consulter son dossier et le valider.</p>
             <p>
                <a href='{validationUrl}' style='padding: 10px 15px; background-color: green; color: white; text-decoration: none; border-radius: 5px;'>✅ Confirmer</a>
                 &nbsp;&nbsp;
                 <a href='{rejectionUrl}' style='padding: 10px 15px; background-color: red; color: white; text-decoration: none; border-radius: 5px;'>❌ Refuser</a>
             </p>
             <p>Merci de votre retour.</p>
            ";

            // TODO: définir le corp des mail envoyé au patient
            string emailBodyPatient = $@"
                <h2>Validation du Dossier Patient</h2>
                <p>votre docier a était valider avec succes vous pouvez revenir a la platfrome pour se connecter !.</p>
            ";

            
            // ✉️ Envoi de l'email au médecin
            bool emailSent = await _emailService.SendEmailAsync(doctorEmail, "Validation du Dossier Patient", emailBody,filePath);
            
            
            if (!emailSent)
            {
                throw new Exception("Échec de l'envoi de l'e-mail au médecin ou pas de validation");
            }

            return Ok(new { message = " e-mail envoyé au médecin pour validation un" });
            //le controlleur validatemdecinresponse.cs se chargera de recupe la reponse du medecin 
        }


        //fin bloc try debut bloc catch 


        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription et du téléchargement du fichier.");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}  




///// BOUT DE CODE ENLEVER A UPLODE A REMETTRE DANS validatemedecinresponse.cs 
/*  else////
                { bool changementvalidationtotrue = _authService.Confirmvalidation(notrepatientmail);

                  if(changementvalidationtotrue == true)
                  {

                     bool emailSentauPatient = await _emailService.SendEmailAsync(notrepatientmail, "Vous etes Validé!", emailBodyPatient);  

                  } 
                 
                  return Ok(new { message = "Patient enregistré, e-mail envoyé au médecin pour validation." });
                }*/