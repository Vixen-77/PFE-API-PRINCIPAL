using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using APIAPP.Data; 
using DotNetEnv; 

var builder = WebApplication.CreateBuilder(args);
// Charger les variables depuis .env
Env.Load();
var appUrl = Env.GetString("API_URL");
var reactAppUrl = Env.GetString("REACT_URL");

if (string.IsNullOrEmpty(appUrl) || string.IsNullOrEmpty(reactAppUrl))
{
    throw new Exception("Les variables d'environnement APP_URL et REACT_APP_URL doivent être définies dans .env");
}

// Utiliser les URLs définies dans .env
builder.WebHost.UseUrls(appUrl);
//  Ajout des services avant builder.Build()
builder.Services.AddControllers(); // tu dit api que tu travail avec des controlleur 
builder.Services.AddHttpClient();//afin de se co a une autre API(secondaire)
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
       policy => policy.WithOrigins(reactAppUrl) // Port où tourne React
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()); // Pour SignalR
});
var app = builder.Build(); //  ICI on fige le service !

// Middleware après builder.Build()
if (app.Environment.IsDevelopment()) //le mode de production mazal
{
    app.UseSwagger();
    app.UseSwaggerUI(); //voir si les requte marche 
}

// app.UseHttpsRedirection(); // (Optionnel, si tu veux éviter le warning)

// Middleware pour gérer les requêtes
app.UseRouting();
app.UseCors("AllowReactApp"); //activation de CORS 
app.UseAuthorization();
app.MapControllers(); // Permet à tes contrôleurs d'être accessibles via l'API
app.MapGet("/", () => "Hello, ASP.NET Core! Répond parfaitement!"); 

app.Run();