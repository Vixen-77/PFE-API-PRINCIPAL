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

[Route("api/auth")]
[ApiController]
public class PatientControlleurSignUP : ControllerBase
{
   private readonly AuthService _authService;
    private readonly ILogger<PatientControlleurSignUP> _logger;
    private readonly string _uploadFolder;

    // 🔹 Constructeur avec injection de dépendances
    public PatientControlleurSignUP(AuthService authService, ILogger<PatientControlleurSignUP> logger)
    {
        _authService = authService;
        _logger = logger;
        _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    }

    [HttpPost("signupPatient")]
    [EnableCors("AllowReactApp")]
    public IActionResult SignUp([FromBody] SignUpPatientRequest request)
    {
      if (request == null)
    {
        return BadRequest("Invalid request");
    }

    bool isRegistered = _authService.SignUpPatient(request);

    if (!isRegistered)
    {
        return Conflict("User already exists with this email.");
    }

    return Ok(new { message = "User registered successfully" });
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
   