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

[Route("api/authRespHop")]
[ApiController]
public class RespHopControlleurSignUP : ControllerBase
{
   private readonly AuthService _authService;
    private readonly ILogger<RespHopControlleurSignUP> _logger;
    private readonly string _uploadFolder;

    // 🔹 Constructeur avec injection de dépendances
    public RespHopControlleurSignUP(AuthService authService, ILogger<RespHopControlleurSignUP> logger)
    {
        _authService = authService;
        _logger = logger;
        _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    }

    [HttpPost("signup")]
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