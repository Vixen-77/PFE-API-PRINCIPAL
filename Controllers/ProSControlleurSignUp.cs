using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LibrarySSMS.Models;
using APIAPP.Services;
using APIAPP.DTO;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class ProSControlleurSignUP : ControllerBase
{
   private readonly AuthService _authService;
    private readonly ILogger<PatientControlleurSignUP> _logger;
   

    // 🔹 Constructeur avec injection de dépendances
    public ProSControlleurSignUP(AuthService authService, ILogger<PatientControlleurSignUP> logger)
    {
        _authService = authService;
        _logger = logger;
    }

[HttpPost("signupProS")]
[EnableCors("AllowReactApp")]
public IActionResult SignUp([FromBody] SignUpProSRequest request)
{
   if (request == null)
    {
        return BadRequest("Invalid request");
    }

    bool isRegistered = _authService.SignUpProSante(request);

    if (!isRegistered)
    {
        return BadRequest(new { message = "User already existe!" });
    }

    return Ok(new { message = "User registered successfully" });
}
}