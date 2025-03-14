using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Cors;
[ApiController]
[Route ("api/react")]

 public class Reactreciver : ControllerBase{
    [HttpGet("json")]
    [EnableCors("AllowReactApp")]
    public IActionResult GetJson()
    {
        var response = new
        {
            Message = "Hello, React! This is your JSON",
            Timestamp = DateTime.UtcNow,
            State = "connexion etablie avec succes"
        };
        
        return Ok(response);
    }
   
 }
 //controlleur a enlver et updater ,