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
using APIAPP.DTO.SignInRaw;
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

    [HttpPost("signinPatient")]
    [EnableCors("AllowReactApp")]
    public IActionResult SignIn([FromForm] SignInRaw request)
    {    
        if (request == null)
        {
            _logger.LogWarning("Requête de connexion invalide : corps de requête null.");
            return BadRequest(new { message = "Données invalides." });
        }

        SignInRequest typedRequest = new ConversionService().ToRaw(request);
        Task <SignInResultt?> result ;

        try
    {
        if (typedRequest.Role == 10)
        {
            result = _authService.SignInPatient(typedRequest.Email, typedRequest.PasswordHash);
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

