using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.DTO;
using APIAPP.DTO.SignUpPatientRawRequest;
using LibrarySSMS.Models;
using APIAPP.Services;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly AppDbContext _context;

        public PatientController(
            AuthService authService,
            ILogger<PatientController> logger,
            AppDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        [HttpPost("signupWithFile")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> SignUpWithFile([FromForm] SignUpPatientRawRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Fichier manquant.");

            SignUpPatientRequest typedRequest = new ConversionService().ToTyped(request);
            SignUpResult? result = await _authService.SignUpPatient(typedRequest);
            if (result == null)
                return Conflict("Cet utilisateur existe déjà.");

            
            // Persister la notification Gmail-like
            var admin = _context.Admins.FirstOrDefault();
            if (admin!= null)  // FIXME:puis rendre fals car les compte admin ne sont pas encore cree 
            {
                var notif = new NotificationAdmin

                {   
                    AdminUID = admin.UID,
                    userUID = result.PatientUID,
                    CreatedAt = DateTime.UtcNow,
                    Message = $"Nouveau patient inscrit : {request.Email}",
                };
                _context.Notificationadmins.Add(notif);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Notif créée pour admin {admin.UID}: {notif.Message}");
            }
            
            return Ok(new { Message = "Inscription réussie.", result.PatientUID });
        }




    
        [HttpGet("notificationsPatient")]
        [EnableCors("AllowReactApp")]
        public IActionResult GetNotifications([FromBody] Guid adminUid)
        {
            var list = _context.Notificationadmins
                .Where(n => n.AdminUID == adminUid)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.userUID,
                    n.Message,
                    n.CreatedAt,
                    n.IsRead
                })
                .ToList();
            return Ok(list);
        }


    


    
        [HttpPost("markAsReadPatient")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> MarkAsRead([FromBody] Guid AdminUID)
       {
        var notif = await _context.Notificationadmins.FindAsync(AdminUID); // ← on cherche la notif par son ID
        if (notif == null)
           return NotFound();

         notif.IsRead = true;
         await _context.SaveChangesAsync();

         return Ok();
        }





       [HttpGet("patientFile")]
       [EnableCors("AllowReactApp")]
       public IActionResult GetPatientFile([FromBody] Guid patientUid)
      {
         // 1. Recherche du patient avec ce UID
        var patient = _context.Patientss.FirstOrDefault(p => p.UID == patientUid);
        if (patient == null)
        return NotFound("Patient introuvable.");

         // 2. Construction du chemin complet vers le fichier du patient
         var fileName = Path.GetFileName(patient.identite); 
         var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Datapatientidf", fileName);

         // 3. Vérification de l'existence du fichier
        if (!System.IO.File.Exists(fullPath))
        return NotFound("Le fichier n'existe pas sur le serveur.");

         // 4. Lecture du fichier et retour en réponse
         var fileBytes = System.IO.File.ReadAllBytes(fullPath);
        return File(fileBytes, "application/octet-stream", fileName); // ← ici on précise le type MIME
      }
   }
}