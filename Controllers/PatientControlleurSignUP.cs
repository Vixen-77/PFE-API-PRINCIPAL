using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LibrarySSMS.Models;
using APIAPP.Services;
using APIAPP.DTO;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly string _uploadFolder;

        public PatientController(AuthService authService, ILogger<PatientController> logger)
        {
            _authService = authService;
            _logger = logger;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        }

        [HttpPost("signupWithFile")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> SignUpWithFile(
            [FromForm] SignUpPatientRequest request,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fichier manquant.");

            // Vérifie si l'utilisateur existe déjà
            var result = _authService.SignUpPatient(request);
            if (!result)
                return Conflict("User already exists.");

            // Création du dossier si non existant
            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);

            // Enregistre le fichier
            var filePath = Path.Combine(_uploadFolder, $"{Guid.NewGuid()}_{file.FileName}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Enregistre le chemin du fichier lié à l'utilisateur
            await _authService.SaveFilePathForUser(request.Email, filePath);//FIXME: rajout de la methode dans servicetierce 

            return Ok("Inscription + upload OK !");
        }
    }
}



//////////////////////////////////////////////////////////////////////////////
/*public IActionResult GetJson()
    {
        var response = new
        {
            Message = "Hello, React! This is your JSON",
            Timestamp = DateTime.UtcNow,
            State = "connexion etablie avec succes"
        };
        
        return Ok(response);
    }*/
   