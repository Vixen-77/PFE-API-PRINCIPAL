using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIAPP.DTO;
using APIAPP.Exceptions;
using APIAPP.DTOResponse;
[ApiController]
[Route("api/auth")]

public class ProSControlleurSignIn : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<PatientControlleurSignIn> _logger;
    

    public ProSControlleurSignIn(AuthService authService, ILogger<PatientControlleurSignIn> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("signinProS")]
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

       Task <SignInResultt?> result ;
        try
    {
        if (request.Role == 20)
        {
            result = _authService.SignInPatient(request.Email, request.PasswordHash);
        }
        else
        {
            return BadRequest(new { message = "Rôle non autorisé pour cette action." });
        }
    }
    catch (AuthException ex)
    {
        _logger.LogWarning($"Échec de l'authentification : {ex.Message}");
        return StatusCode(ex.StatusCode, new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erreur serveur lors de la connexion.");
        return StatusCode(500, new { message = "Erreur serveur." });
    }

    return Ok(result);
        
    }
}

