using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LibrarySSMS.Models;
using APIAPP.Services;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibrarySSMS;
using LibrarySSMS.Enums;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
   private readonly AppDbContext _context;

   public AuthController(AppDbContext dbContext)
   {
    _context = dbContext;
   }

    [HttpGet("checkUser/{email}")]
    public async Task<IActionResult> CheckUserExists(string email)
    {
        // Affiche l'email reçu dans la console pour le debug
        Console.WriteLine($"🔍 Vérification de l'utilisateur avec l'email : {email}");

        // Recherche de l'utilisateur dans la base
        var user = await _context.ProSs
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user != null)
        {
            Console.WriteLine($"✅ Utilisateur trouvé : {user.FullName}");
            return Ok(new { message = "Utilisateur trouvé", user });
        }
        else
        {
            Console.WriteLine("❌ Aucun utilisateur trouvé !");
            return NotFound(new { message = "Aucun utilisateur trouvé avec cet email." });
        }
    }
}
