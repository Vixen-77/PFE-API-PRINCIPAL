using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Models;
using APIAPP.Services;
using APIAPP.DTO;
using APIAPP.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;

/*[Route("api/auth")]
[ApiController]
public class UserControlleurSignUP : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<UserControlleurSignUP> _logger;
    private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public UserControlleurSignUP(AuthService authService, ILogger<UserControlleurSignUP> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("signup")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> SignUp([FromForm] string name, [FromForm] string email, [FromForm] int age, [FromForm] string role, [FromForm] IFormFile file)
    {
        // Vérifier si le rôle est valide
        if (string.IsNullOrEmpty(role) || !Enum.TryParse<RoleManager>(role, true, out var userRole))
            return BadRequest("Le rôle fourni est invalide.");

        // Vérifier le fichier
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Aucun fichier sélectionné" });

        try
        {
            // Vérifier si le dossier d'upload existe
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }

            // Générer un nom de fichier unique
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_uploadFolder, uniqueFileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Création de l'objet patient
            var request = new SignUpPatientRequest
            {
                 
            };

            // Inscription selon le rôle
            bool result = userRole switch
            {
                RoleManager.Patient => _authService.SignUpPatient( SignUpPatientRequest request),
                RoleManager.ProfSanté => _authService.SignUpProSante(SignUpProSRequest request),
                RoleManager.RespHop => _authService.SignUpRespoHopital(SignUpRespHopRequest request),
                _ => false
            };

            if (!result)
            {
                _logger.LogError("Échec de l'inscription pour l'email : {Email}", email);
                return BadRequest("L'inscription a échoué.");
            }

            return Ok(new
            {
                message = "Inscription réussie.",
                name,
                email,
                age,
                role,
                imageUrl = request.ProfilePictureUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Erreur lors de l'inscription : {Error}", ex.Message);
            return StatusCode(500, new { message = "Erreur interne", error = ex.Message });
        }
    }
}*/