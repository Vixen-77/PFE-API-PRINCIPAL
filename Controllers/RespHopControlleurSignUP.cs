using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using APIAPP.DTO;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class RespHopControlleurSignUP : ControllerBase
{
   private readonly AuthService _authService;
    private readonly ILogger<RespHopControlleurSignUP> _logger;

    // 🔹 Constructeur avec injection de dépendances
    public RespHopControlleurSignUP(AuthService authService, ILogger<RespHopControlleurSignUP> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("signupRespHop")]
    [EnableCors("AllowReactApp")]
    public IActionResult SignUp([FromBody] SignUpRespHopRequest request)
    {
    // Add your implementation here + rajout de l'apple de service 
       if (request == null)
    {
        return BadRequest("Invalid request");
    }

    bool isRegistered = _authService.SignUpRespoHopital(request);

    if (!isRegistered)
    {
        return Conflict("User already exists with this email.");
    }

    return Ok(new { message = "User registered successfully" });
    }
}