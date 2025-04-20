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

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ProSController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly AppDbContext _context;

        public ProSController(
            AuthService authService,
            ILogger<PatientController> logger,
            AppDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        [HttpPost("signupWithFileProS")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> SignUpWithFile([FromForm] SignUpProSRequest request)
        {
            if (request.File == null || request.File.Length == 0 ||request.FileCertif == null || request.FileCertif.Length == 0)
                return BadRequest("soit le fichier identité est manquant ou bien celui de certification.");


            SignUpResultProS resultP = await _authService.SignUpProSante(request);
            if (resultP == null)
                return Conflict("Cet utilisateur existe déjà.");

            // Persister la notification Gmail-like
            var admin = _context.Admins.FirstOrDefault();
            if (admin != null)
            {
                var notif = new NotificationAdmin

                {   Id = int.Parse(Nanoid.Generate(size: 6).Substring(0, 9)),// Ensure the string is converted to an integer
                    AdminUID = admin.UID,
                    PatientUID = resultP.ProSUID,
                    CreatedAt = DateTime.UtcNow,
                    Message = $"Nouveau patient inscrit : {request.Email}",
                };
                _context.Notificationadmins.Add(notif);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Notif créée pour admin {admin.UID}: {notif.Message}");
            }

            return Ok(new { Message = "Inscription réussie.", resultP.ProSUID });
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
                    n.PatientUID,
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