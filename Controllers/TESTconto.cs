using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/test")]
public class TESTconto : ControllerBase
{
    [HttpGet("serialize")]
    public IActionResult SerializeObject()
    {
        var obj = new { Nom = "RAIDER", Projet = "API" };
        string json = JsonSerializer.Serialize(obj);
        return Content(json, "application/json");
    }
}
