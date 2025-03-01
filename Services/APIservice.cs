using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using APIAPP.Models; 

public class TestService
{
    private readonly HttpClient _httpClient;

    public TestService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TestEntity>> GetTestsFromPrimaryAPI()
    {
        var response = await _httpClient.GetAsync("http://localhost:5002/api/test"); // URL de l'API principale

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TestEntity>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TestEntity>();
        }

        return new List<TestEntity>();
    }
}
