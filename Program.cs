using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using LibrarySSMS; 
using APIAPP.Services;
using DotNetEnv; 
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Extensions;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// Charger les variables depuis .env
Env.Load("C:\\Users\\HP\\Documents\\L3\\PFE\\PFE-API-PRINCIPAL\\.env");
var apiUrl = Env.GetString("API_URL");
var reactAppUrl = Env.GetString("REACT_URL");
var ipconfig = Env.GetString("IP_ADR");


if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(reactAppUrl))
{
    throw new Exception("Les variables d'environnement APP_URL et REACT_APP_URL doivent être définies dans .env");
}

// Utiliser les URLs définies dans .env
builder.WebHost.UseUrls(apiUrl);

// Ajout des services avant builder.Build()
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\HP\Documents\L3\PFE\PFE-API-PRINCIPAL"))    .SetApplicationName("MonApplication");


    //ingnoré le warning du chiffrement 
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddScoped<AuthService>(); 
builder.Services.AddScoped<IEmailService, EmailService>(); // 🛠️ Enregistrer IEmailService
builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<NotificationStore>();  // Enregistrement du NotificationStore
builder.Services.AddScoped<GlobalService>();
builder.Services.AddScoped<ISmsService,SmsService>();
builder.Services.AddScoped<UploaderPatient>();          
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Logging.AddConsole();


// Configuration JWT (📌 Ajouté ici)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Env.GetString("JWT_SECRETE");
Console.WriteLine($"Clé secrète récupérée : {secretKey}");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Ajout de l'autorisation (📌 Obligatoire après l'authentification)
builder.Services.AddAuthorization();

// Ajout du service JWT (📌 Permet de générer des tokens)
builder.Services.AddSingleton<JWTService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins($"http://{ipconfig}:3001",$"http://{ipconfig}:8081")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

         var app = builder.Build(); // ICI on fige le service !

       /*  var scope = app.Services.CreateScope();
         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Appliquer les migrations automatiquement
         context.Database.Migrate();
           var newPro = new ProS
        {
            UID = Guid.NewGuid(),
            FullName = "John Doe", //1
            Email = "test@example.com", //2
            PasswordHash = "simple", //3
            Salt = "salttest",
            Role = RoleManager.ProfSanté, //4
            IsActive = true,
            City = "Paris", //5
            PostalCode = "75000", //6
            DateOfBirth = new DateTime(1990, 5, 15), //7
            PhoneNumber = "+33612345678", //8
            CreatedAt = DateTime.UtcNow,
            AccountStatus = false,
            SubscriptionPlan = true,
            IsAvailable = true,
            AcceptRequest = true,
            CheckedSchedule = true,
            TwoFactorEnabled = false,
            IsOnline = false,
            LastLogin = DateTime.UtcNow
        };
        Console.WriteLine("Insertion en cours...");
        context.ProSs.Add(newPro);
        context.SaveChanges();
        Console.WriteLine("Insertion réussie !");*/

// Middleware après builder.Build()
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowReactApp"); 
app.UseAuthentication();// Doit être avant l'auth
app.UseAuthorization(); // Bien placé avant MapControllers
app.MapControllers(); // Plus besoin de UseEndpoints !
//app.UseStaticFiles(); // dans Program.cs useless
app.UseDeveloperExceptionPage();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Data")),
    RequestPath = "/Data"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "ProfilePicPatient")),
    RequestPath = "/ProfilePicPatient"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Datapatientidf")),
    RequestPath = "/Datapatientidf"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "DataProSCertif")),
    RequestPath = "/DataProSCertif"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "DataProSidf")),
    RequestPath = "/DataProSidf"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "DataProSidf")),
    RequestPath = "/ProfilePicPatient"
});

app.MapGet("/", () => "Hello, ASP.NET Core! Répond parfaitement!");

app.Run();














































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