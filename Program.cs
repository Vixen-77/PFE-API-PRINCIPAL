using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using APIAPP.Data; 
using DotNetEnv; 
using APIAPP.Models;
using APIAPP.Enums;

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

// Ajout des services avant builder.Build()
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
       policy => policy.WithOrigins(reactAppUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build(); // ICI on fige le service !

// **Ajout de données après app.Build()**
/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Appliquer les migrations automatiquement
    context.Database.Migrate();

    // Vérifier si des données existent déjà
    if (!context.TestEntities.Any()) 
    {
        context.TestEntities.Add(new TestEntity
        {
            Name = "John Doe",
            Etat = UserState.Conducteur,
            Username = "johndoe",
            Email = "johndoe@example.com",
            PasswordHash = "hashedpassword",
            Salt = "randomsalt",
            FullName = "John Doe",
            City = "Sample City",
            PostalCode = "12345",
            PhoneNumber = "123-456-7890",
            AccountStatus = AccountStatus.Active,
            Device = Device.SmartCarOBU,
            SubscriptionPlan = true,
            TwoFactorEnabled = false
        });

        context.SaveChanges();
    }
}*/

// Middleware après builder.Build()
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Hello, ASP.NET Core! Répond parfaitement!");

app.Run();
