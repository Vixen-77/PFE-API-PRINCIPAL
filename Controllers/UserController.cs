/*using Microsoft.AspNetCore.Mvc;
using APIAPP.Models;
using APIAPP.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] SignInRequest request)
    {
        string token = request.Role switch
        {
            "Patient" => _authService.SignInPatient(request.Email, request.Password),
            "ProSante" => _authService.SignInProSante(request.Email, request.Password),
            "RespoHopital" => _authService.SignInRespoHopital(request.Email, request.Password),
            _ => null
        };

        if (token == null)
            return Unauthorized("Email ou mot de passe incorrect.");

        return Ok(new { Token = token });
    }

    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] SignUpRequest request)
    {
        bool result = request.Role switch
        {
            "Patient" => _authService.SignUpPatient(request),
            "ProSante" => _authService.SignUpProSante(request),
            "RespoHopital" => _authService.SignUpRespoHopital(request),
            _ => false
        };

        if (!result)
            return BadRequest("L'inscription a échoué.");

        return Ok("Inscription réussie.");
    }
}*/
