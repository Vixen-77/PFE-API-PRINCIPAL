using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Models;
using APIAPP.DTO;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Cors;

[ApiController]
[Route("api/passwrodreset")]
public class PasswordResetController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly ILogger<PasswordResetController > _logger;

    // 🔹 Constructeur avec injection de dépendances
    public PasswordResetController(AuthService authService, ILogger<PasswordResetController > logger, GlobalService globalService, EmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _globalService = globalService; // Assurez-vous que GlobalService est correctement initialisé
    }

       
   [HttpPost("requestcode")]
   [EnableCors("AllowReactApp")]
   public async Task<IActionResult> RequestCode([FromForm] PasswordRequestDto request)
   {
        switch (request.Role)
       {
        case "10": // Patient
            if (!await _globalService.VerifyPasswordPatient(request.Email))
            {
                throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
            }
            break;

        case "20": // Professionnel de santé
            if (!await _globalService.VerifyPasswordProS(request.Email))
            {
                throw new AuthException("Votre compte n'existe pas ou bien Email ou mot de passe incorrect.", 401);
            }
            break;

        default:
            return BadRequest("Rôle non reconnu.");
       }

        var code = _globalService.GenerateCode(request.Email);

        string head ="<h1>Code de réinitialisation de mot de passe</h1>";
        string body = $"<h1>Code de réinitialisation de mot de passe</h1><p>Votre code est :</p>";

        var good = await _emailService.SendCodeEmail(request.Email, head, body, code);
        if (!good)
        {
        return BadRequest("Erreur lors de l'envoi de l'email.");
        }
   
         return Ok("Un code a été envoyé à votre email");
    }




    [HttpPost("submit-code")]
    [EnableCors("AllowReactApp")]
    public IActionResult SubmitCode([FromForm] CodeSubmissionDto submission)
    {
      bool valid = _globalService.ValidateCode(submission.Email, submission.Code);
     return valid 
    ? Ok("Code correct, tu peux changer ton mot de passe") 
    : BadRequest("Code invalide");
    }
}