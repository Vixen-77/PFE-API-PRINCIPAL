using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/test1")]
public class TESTconto : ControllerBase
{
private readonly HttpClient _httpClient;
public TESTconto(IHttpClientFactory httpClientFactory)
{
    _httpClient = httpClientFactory.CreateClient();
}
     
[HttpGet("serialize")]
public async Task<IActionResult> Serialize([FromQuery] bool isSender = true)
{
    if (isSender)
    {
        // Mode "Expéditeur" -> Envoie la requête à TestC
        var response = await _httpClient.GetAsync("http://localhost:5002/api/test2/serialize?isSender=false");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Erreur lors de l'appel à l'API secondaire.");
        }

        string result = await response.Content.ReadAsStringAsync();
        return Ok($"Réponse de l'API B : {result}");
    }
    else
    {
        // Mode "Récepteur" -> Retourne une simple réponse
        return Ok("Réponse de l'API principale !");
    }
}
}