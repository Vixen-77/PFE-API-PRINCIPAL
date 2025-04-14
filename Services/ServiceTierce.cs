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
using Microsoft.AspNetCore.Http.HttpResults;

namespace APIAPP.Services
{
       public class SercviceTierce   
    {
    private readonly AppDbContext _dbContext;         
    private readonly IEmailService _emailService;
    private readonly UploadindatabaseService _uploaddatabase;
    
    private readonly ILogger<UploaderPatient> _logger;

    public SercviceTierce( IEmailService emailService,UploadindatabaseService uploadindatabase,AuthService authService,AppDbContext dbContext,ILogger<UploaderPatient> logger)
    {
        _uploaddatabase = uploadindatabase;
        _emailService = emailService;
        _dbContext = dbContext; // Correction ici
        _logger = logger;
    }

    public async Task<bool> SignupAndUpload(IFormFile file, [FromForm] Guid userId)

    {
        if (file == null || file.Length == 0)
            return false;

        var uploadFolder = Path.Combine("C:\\Users\\ASUS\\Desktop\\PFE3.0\\APIprincipal\\APIAPP\\Datapatientidf");
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
            var admin = await   _dbContext.Admins.FindAsync(userId);
            

            if (admin == null)
            {
                return false;
            }

            string adminEmail = admin.MailMed;
            string  notrepatientmail = patient.Email;
            if (string.IsNullOrEmpty(doctorEmail))
            {
                return false;
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

            return true;
            //le controlleur validatemdecinresponse.cs se chargera de recupe la reponse du medecin 
        }


        //fin bloc try debut bloc catch 


        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription et du téléchargement du fichier.");
            return false;
        }
    }

    public async Task<bool> Upload(IFormFile file, [FromForm] Guid userId)

    {
        if (file == null || file.Length == 0)
            return false;

        var uploadFolder = Path.Combine("C:\\Users\\ASUS\\Desktop\\PFE3.0\\APIprincipal\\APIAPP\\Datapatientidf");
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
                return false;
            }

            string doctorEmail = patient.MailMed;
            string  notrepatientmail = patient.Email;
            if (string.IsNullOrEmpty(doctorEmail))
            {
                return false;
            }

            // 📧 Génération du contenu de l'e-mail
            string validationUrl = $"http://localhost:5001/api/uploadsPatient/validate?userId={userId}&approve=true";
            string rejectionUrl = $"http://localhost:5001/api/uploadsPatient/validate?userId={userId}&approve=false";





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

            return true;
            //le controlleur validatemdecinresponse.cs se chargera de recupe la reponse du medecin 
        }


        //fin bloc try debut bloc catch 


        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription et du téléchargement du fichier.");
            return false;
        }
    }
  }
}