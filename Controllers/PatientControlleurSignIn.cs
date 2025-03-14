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
using Microsoft.EntityFrameworkCore;
using APIAPP.Data;




[ApiController]
[Route("api/auth")]

public class PatientControlleurSignIn : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<PatientControlleurSignIn> _logger;
    

    // 🔹 Constructeur avec injection de dépendances
    public PatientControlleurSignIn(AuthService authService, ILogger<PatientControlleurSignIn> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("signin")]
    [EnableCors("AllowReactApp")]
    public IActionResult SignIn([FromBody] SignInRequest request)
    {    
        if (request == null)
        {
            _logger.LogWarning("Requête de connexion invalide : corps de requête null.");
            return BadRequest(new { message = "Données invalides." });
        }

        // Vérification que le rôle reçu est bien défini dans RoleManager
        if (!Enum.IsDefined(typeof(RoleManager), request.Role))
        {
            _logger.LogWarning($"Rôle invalide reçu : {request.Role}");
            return BadRequest(new { message = "Rôle invalide." });
        }

          string token = string.Empty;

        // 🔹 Sélection du service selon le rôle
       
        if (request.Role == 10) {token = _authService.SignInPatient(request.Email, request.PasswordHash,request.Validation); }
        // 🔹 Si l'authentification échoue"
        if (token == null)
        {
            _logger.LogWarning("Échec de l'authentification pour {Email}", request.Email);
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });
        }

        // 🔹 En cas de succès, on renvoie un JSON vers React
        _logger.LogInformation("Utilisateur {Email} authentifié avec succès en tant que {Role}.", request.Email, request.Role);
        return Ok(new
        {
            message = "Authentification réussie",
            role = request.Role,
            data = token  // Contient potentiellement un token, le nom de l'utilisateur, etc.
        });
    }
}


/*
 {
  "email"= valeur 
 
  "mdp"= valeur 

  "role"= valeur 
 
 }



*/