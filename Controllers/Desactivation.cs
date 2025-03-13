/*using Microsoft.AspNetCore.Mvc;
using APIAPP.Models;
using APIAPP.Services;
using APIAPP.DTO;
using Microsoft.AspNetCore.Cors;

[ApiController]
[Route("api/desactivation")]
public class DesactivationController : ControllerBase
{
    private readonly AuthService _authService;

    public DesactivationController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("desactivation")]
    [EnableCors("AllowReactApp")]
    public IActionResult DesactivateAccount([FromBody] Desactivation request)
    {
        if (request.Email == null)
        {
            return BadRequest("L'email est requis.");
        }

        bool result = _authService.DesactivateAccount(request.Email);
        if (!result)
        {
            return BadRequest("La désactivation a échoué.");
        }

        return Ok("Compte désactivé avec succès.");
    }
}*/