using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[ApiController]
[Route ("api/json")]

 public class JSONcreation : ControllerBase
{

    [HttpGet("json")]
    public IActionResult GetJson()
    {
        var response = new
        {
            Message = "Hello, Hiki!",
            Status = "Success",
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }
}

