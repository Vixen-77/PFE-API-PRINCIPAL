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
using Microsoft.OpenApi.Extensions;

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly AppDbContext _context;
        private readonly object? _monip;

        public PatientController(
            AuthService authService,
            ILogger<PatientController> logger,
            AppDbContext context,
            EmailService emailService,
            IConfiguration configuration)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _monip = configuration["ipadr"] ?? throw new ArgumentException("ip manquant dans la configuration.");
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
            var id = result.PatientUID.ToString();
            if(id != null)
            {
            if (result.email == null) {
                return BadRequest("Email manquant.");
            }
            var role = "10";
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
    
            var isSent = await _emailService.SendEmailAsyncValidation(result.email, subject, body);
            
            //choix entre admin:0 et super admin:1
            Random random = new Random();
            int choix= random.Next(0, 2); // 0 ou 1
                choix = 0; //FIXME: A ENLEVER APRES AVOIR CREE SUPERADMIN
            if(choix==0){
            //choix d un admin random 

            var admin = _context.Admins
                        .OrderBy(a => Guid.NewGuid())
                        .FirstOrDefault();

            if (admin != null)  // FIXME:puis rendre fals car les compte admin ne sont pas encore cree 
            {
                var CreaCompte = new CreationCompte
                {
                    Id = Guid.NewGuid(),
                    UserUID = Guid.Parse(id),
                    Role="10",
                    State=0,
                    AdminUID=admin.UID,
                    CreatedAt= DateTime.UtcNow,
                    
                };
                _context.CreationCompte.Add(CreaCompte);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Notif créée pour admin {CreaCompte.AdminUID}: patient {CreaCompte.UserUID}");
            }
            }
            else{
                //choix d un super admin random 
                var superAdmin = _context.SuperAdmins
                        .OrderBy(a => Guid.NewGuid())
                        .FirstOrDefault();

                    if (superAdmin != null)  // FIXME:puis rendre fals car les compte admin ne sont pas encore cree 
                    {
                        var CreaCompte = new CreationCompte
                        {
                            Id = Guid.NewGuid(),
                            UserUID = Guid.Parse(id),
                            Role = "10",
                            State = 0,
                            AdminUID = superAdmin.UID,
                            CreatedAt = DateTime.UtcNow,

                        };
                        _context.CreationCompte.Add(CreaCompte);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Notif créée pour superadmin {CreaCompte.AdminUID}: patient {CreaCompte.UserUID}");
                
                }
            }
            if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email de confirmation.");
            return Ok(new { Message = "Inscription réussie.", result.PatientUID });
            }
            else
            {
                return BadRequest("Erreur lors de l'inscription.");
            }
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