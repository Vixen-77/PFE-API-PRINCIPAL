using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
// TODO: ce code gère l'interconnection avec API secondaire le code se divise en deux partie isSender = true (exporteur)et isSender = false(receveur)
//FIXME: LE code changera au fure et a mesure si les deux API doivent faire des tache de comunication bien spésifique mais la base de ce code reste la meme 
//NOTE: le Ce controlleur a le meme nom que le controlleur dans API secondaire la differance est dans URL de distination 
 //TODO: 5001-->5002 et       (5002-->5001 dans API secondaire)

[ApiController]
[Route("api/call1")]
public class APIcall : ControllerBase
{
private readonly HttpClient _httpClient;
public APIcall(IHttpClientFactory httpClientFactory)
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
