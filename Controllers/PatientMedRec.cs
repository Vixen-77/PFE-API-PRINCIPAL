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

[Route("api/validationmail")]
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
    public async Task<IActionResult> UploadPatient([FromForm] MedRecRaw request)
    {
        if (request.file == null || request.file.Length == 0)
            return BadRequest("Fichier non fourni ou vide.");
        // Appel au service pour uploader le fichier et envoyer l'email
        pathandID? thepathishere = await _methodePatientService.UploadandEmail(request.file, Guid.Parse(request.ID), request.MailMedecin, request.Title);

        if (thepathishere == null)
            return BadRequest("Erreur lors de l'upload du fichier.");
        else {
        {  

          string baseUrl = "http://192.168.1.102:5001";
          string subject = "Nouveau dossier médical reçu: "+" "+request.Title;
          string dossierId = thepathishere.ID.ToString();
          string body = $@"
    <html>
      <body>
        <p>Bonjour,</p>
        <p>Vous avez reçu un nouveau dossier médical concernant un patient.</p>
        <p>
          <a href='{baseUrl}/api/validationmail/accept?dossierId={dossierId}' 
             style='padding:10px 20px; background-color:green; color:white; text-decoration:none; border-radius:5px;'>
             ✔️ Valider
          </a>
        </p>
        <p>
          <a href='{baseUrl}/api/validationmail/refuse?dossierId={dossierId}' 
             style='padding:10px 20px; background-color:red; color:white; text-decoration:none; border-radius:5px;'>
             ❌ Refuser
          </a>
        </p>
        <p>Merci de votre vigilance.</p>
      </body>
    </html>";

            var isSent = await _emailService.SendEmailAsync(request.MailMedecin, subject, body, thepathishere.path.ToString());
            if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email.");
        }
        }

       var response = new
            {
                Message = "Dossier médical validé avec succès.",
                idfdossier = thepathishere.ID,
            };
            return Ok(response);
        
        }
    











        [HttpGet("accept")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> Accept(string dossierId)
        {
            var Toguiddoc = Guid.Parse(dossierId);
            var MedRec = await _context.MedRecs.FirstOrDefaultAsync(p => p.UIDMedRec == Toguiddoc);
            if (MedRec == null)
            {
                return NotFound("Dossier médical non trouvé.");
            }
            if (MedRec.State!= MedRecState.Pending){
                return BadRequest("Le dossier médical a déjà été traité.");
            }
            
            MedRec.State = MedRecState.valid; // Mettre à jour l'état du dossier médical
            await _context.SaveChangesAsync();
            
    // 2. Trouver le patient correspondant
           var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == MedRec.PatientUID);

         if (patient == null)
        {
        return NotFound("Patient non trouvé.");
        }


     // 4. Tu peux ici utiliser patient.Email si besoin
            string patientEmail = patient.Email;
            string subject = "Dossier médical validé";
            string body = $@"Votre dossier médical a été validé avec succès ! 
            Nous garderons une trace de votre santé pour intervenir rapidement en cas d'urgence. Merci.";


            var isSent = await _emailService.SendEmailAsyncValidation(patientEmail, subject, body);
            if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email.");
    
            return Ok("Dossier validé. Merci !");
        }







       [HttpGet("refuse")]
       [EnableCors("AllowReactApp")]

        public  async Task<IActionResult> Refuse(string dossierId)
        {
        var Toguiddoc = Guid.Parse(dossierId);
        var MedRec= await _context.MedRecs.FirstOrDefaultAsync(p => p.UIDMedRec == Toguiddoc);
         if (MedRec == null)
            {
                return NotFound("Dossier médical non trouvé.");
            }
            if (MedRec.State!= MedRecState.Pending){
                return BadRequest("Le dossier médical a déjà été traité.");
            }
            MedRec.State = MedRecState.unvalid; // Mettre à jour l'état du dossier médical
            await _context.SaveChangesAsync();
            // 2. Trouver le patient correspondant
           var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == MedRec.PatientUID);

         if (patient == null)
        {
        return NotFound("Patient non trouvé.");
        }
     // 4. Tu peux ici utiliser patient.Email si besoin
            string patientEmail = patient.Email;
            Guid MedRecUid = MedRec.UIDMedRec;
            string subject = "Dossier médical Refusé";
            string body = $@"Votre dossier médical a été refusé. 
            Veuillez contacter votre médecin pour plus de détails. Merci pour votre compréhension.";

            var isSent = await _emailService.SendEmailAsyncValidation(patientEmail, subject, body);
            if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email.");
    
            return Ok("Dossier Refusé. Merci !");

        }



        [HttpPost("recupListMedRec")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> RecupeMedrec([FromBody]string patient)
        {
        if (!Guid.TryParse(patient, out Guid patientId))
            {
            return BadRequest("ID de patient invalide.");
            }

        var medRecs = await _context.MedRecs
            .Where(n => n.PatientUID == patientId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
            n.UIDMedRec,
            n.PatientUID,
            filePath = "http://192.168.1.102:5001/" + n.FilePath.Replace("\\", "/"),
            n.CreatedAt,
            n.State,
            n.MailMed,
            n.Title,
            n.Description,
            })
            .ToListAsync();

         if (medRecs == null || !medRecs.Any())
            {
            return Ok("Aucun document médical trouvé pour ce patient.");
        }

        return Ok(medRecs);
    }

    [HttpPost("deleteListMedRec")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> DelectionMedRec([FromBody] DeleteMedRecRequest request)
{
    var idfPatient = Guid.Parse(request.Patient);
    var idfDossier = Guid.Parse(request.IdDossier);

    var medicalREC = await _context.MedRecs
        .FirstOrDefaultAsync(p => p.PatientUID == idfPatient && p.UIDMedRec == idfDossier);

    if (medicalREC == null)
    {
        return NotFound("Aucun dossier médical correspondant n'a été retrouvé.");
    }

    _context.MedRecs.Remove(medicalREC);
    await _context.SaveChangesAsync();

    return Ok("Dossier médical supprimé avec succès.");
}


    }