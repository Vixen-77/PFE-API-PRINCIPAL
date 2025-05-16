using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.DTO;
using LibrarySSMS.Models;
using APIAPP.Services;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NanoidDotNet;
using APIAPP.DTO.SignUpProSRawRequest;

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ProSController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly AppDbContext _context;

        public ProSController(
            AuthService authService,
            ILogger<PatientController> logger,
            AppDbContext context,
            EmailService emailService)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("signupWithFileProS")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> SignUpWithFile([FromForm] SignUpProSRawRequest request)
        {
            if (request.File == null || request.File.Length == 0 ||request.FileCertif == null || request.FileCertif.Length == 0)
                return BadRequest("soit le fichier identité est manquant ou bien celui de certification.");

            SignUpProSRequest typedRequest = new ConversionService().ToTypedProS(request);
            SignUpResultProS resultP = await _authService.SignUpProSante(typedRequest);
            if (resultP == null)
                return Conflict("Cet utilisateur existe déjà.");

        var id= resultP.ProSUID.ToString();
            if (id != null)
            {
                if (resultP.email == null)
                {
                    return BadRequest("Email manquant.");
                }
                var role = "20";
                // Envoi d'un email de confirmation
                string baseUrl = "http://192.168.1.102:5001";
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
                var isSent = await _emailService.SendEmailAsyncValidation(resultP.email, subject, body);

                //choix entre admin:0 et super admin:1
                Random random = new Random();
                int choix = random.Next(0, 2); // 0 ou 1

                if (choix == 0)
                {
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
                            Role = "20",
                            State = 0,
                            AdminUID = admin.UID,
                            CreatedAt = DateTime.UtcNow,

                        };
                        _context.CreationCompte.Add(CreaCompte);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Notif créée pour admin {CreaCompte.AdminUID}: proS {CreaCompte.UserUID}");
                    }
                }
                else
                {
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
                            Role = "20",
                            State = 0,
                            AdminUID = superAdmin.UID,
                            CreatedAt = DateTime.UtcNow,

                        };
                        _context.CreationCompte.Add(CreaCompte);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Notif créée pour superadmin {CreaCompte.AdminUID}: proS {CreaCompte.UserUID}");

                    }
                }
                if (!isSent) return BadRequest("Erreur lors de l'envoi de l'email de confirmation.");
                return Ok(new { Message = "Inscription réussie.", resultP.ProSUID });
            }
            else
            {
                return BadRequest("Erreur lors de l'inscription.");
            }
        }





        [HttpGet("notificationsProS")]
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






        [HttpPost("markAsReadProS")]
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





       [HttpGet("ProSFile")]
       [EnableCors("AllowReactApp")]
       public IActionResult GetProSFile([FromBody] Guid proStUid)
{
    var proS = _context.ProSs.FirstOrDefault(p => p.UID == proStUid);
    if (proS == null)
        return NotFound("Patient introuvable.");

    var fileName = Path.GetFileName(proS.identite);
    var fileName2 = Path.GetFileName(proS.Certif);

    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "DataProSidf", fileName);
    var fullPath2 = Path.Combine(Directory.GetCurrentDirectory(), "DataProSCertif", fileName2);

    if (!System.IO.File.Exists(fullPath) || !System.IO.File.Exists(fullPath2))
        return NotFound("Un ou plusieurs fichiers sont introuvables.");

    using (var memoryStream = new MemoryStream())
    {
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var entry1 = archive.CreateEntry(fileName);
            using (var entryStream = entry1.Open())
            using (var fileStream = System.IO.File.OpenRead(fullPath))
            {
                fileStream.CopyTo(entryStream);
            }

            var entry2 = archive.CreateEntry(fileName2);
            using (var entryStream2 = entry2.Open())
            using (var fileStream2 = System.IO.File.OpenRead(fullPath2))
            {
                fileStream2.CopyTo(entryStream2);
            }
        }

         var nom = proS.LastName?.Trim().Replace(" ", "_");
         var prenom = proS.Name?.Trim().Replace(" ", "_");

         var zipName = $"dossier_{nom}_{prenom}.zip";
        return File(memoryStream.ToArray(), "application/zip", zipName);
    }
}
   }
}