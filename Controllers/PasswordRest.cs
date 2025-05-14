using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Models;
using APIAPP.DTO;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Cors;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/passwrodreset")]
public class PasswordResetController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;

    private readonly ILogger<PasswordResetController > _logger;

    // 🔹 Constructeur avec injection de dépendances
    public PasswordResetController(AuthService authService, ILogger<PasswordResetController > logger, GlobalService globalService, EmailService emailService, AppDbContext context)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _globalService = globalService; // Assurez-vous que GlobalService est correctement initialisé
        _context = context;

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
                throw new AuthException("Email incorrect.", 401);
            }
            break;

        case "20": // Professionnel de santé
            if (!await _globalService.VerifyPasswordProS(request.Email))
            {
                throw new AuthException("Email incorrect.", 401);
            }
            break;

        default:
            return BadRequest("Rôle non reconnu.");
       }

        var code = _globalService.GenerateCode(request.Email);

        string head ="Code de réinitialisation de mot de passe";
        string body = $"<h3>Code de réinitialisation de mot de passe</h3><p>Votre code est :</p>";

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
    ? Ok("Code correct") 
    : BadRequest("Code invalide");
    }

    [HttpPost("nvmdp")]
    [EnableCors("AllowReactApp")]
     public async Task<IActionResult> Newpwd([FromForm] NewPWD submission)
    {
      
      var UserID=Guid.Parse(submission.Uid);

                if (submission.Role == "10")
                {
                    var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == UserID);
                    if (patient != null)
                    {   
                        patient.Salt = _authService.GenerateSalt();
                        patient.PasswordHash = _authService.HashPassword(submission.NewPassword, patient.Salt);
                        await _context.SaveChangesAsync();
                        return Ok("Mot de passe modifié avec succès.");
                    }
                }
                else if (submission.Role == "20")
                {
                    var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == UserID);
                    if (proS != null)
                    {
                       
                        proS.Salt = _authService.GenerateSalt();
                        proS.PasswordHash = _authService.HashPassword(submission.NewPassword, proS.Salt);
                        await _context.SaveChangesAsync();
                        return Ok("Mot de passe modifié avec succès.");
                    }
                }

        return BadRequest("Une erreur s'est produite lors de la soumission du code.");
    }

    
}

