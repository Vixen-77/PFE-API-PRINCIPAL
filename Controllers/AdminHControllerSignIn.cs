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
using APIAPP.DTOResponse;
using APIAPP.Exceptions;

[ApiController]
[Route("api/auth")]

public class AdminHSignIn : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AdminSignIn> _logger;
    

    // 🔹 Constructeur avec injection de dépendances
    public AdminHSignIn(AuthService authService, ILogger<AdminSignIn> logger)
    {
        _authService = authService;
        _logger = logger;
    }

[HttpPost("signinAdminHopitale")]
[EnableCors("AllowReactApp")]
public IActionResult SignIn([FromForm] SignInRequestAdmin request)
{    
    if (request == null)
    {
        _logger.LogWarning("Requête de connexion invalide : corps de requête null.");
        return BadRequest(new { message = "Données invalides." });
    }

    if (!Enum.IsDefined(typeof(RoleManager), request.Role))
    {
        _logger.LogWarning($"Rôle invalide reçu : {request.Role}");
        return BadRequest(new { message = "Rôle invalide." });
    }

    // 🔹 Initialisation de la variable result
    Task <SignInResultAdmin?> result ;

    try
    {
        if (request.Role == 30)
        {
            result = _authService.SignInAdminH(request.Email, request.PasswordHash, request.UIDKEY);
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

