using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIAPP.DTO;
using APIAPP.DTOResponse;
using APIAPP.Exceptions;
using LibrarySSMS.Models;
using LibrarySSMS;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("api/auth")]

public class AddAdmin : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AddAdmin> _logger;

    private readonly AppDbContext _context;

    // 🔹 Constructeur avec injection de dépendances
    public AddAdmin(AuthService authService, ILogger<AddAdmin> logger, AppDbContext context)
    {
        _authService = authService;
        _logger = logger;
        _context = context;
    }

    [HttpPost("AddAdmin")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> AddAdminnAsync([FromForm] string Email, [FromForm] string Password, [FromForm] string Fullname, [FromForm] string Phonenumber)
    {

        if (_context.Admins.Any(p => p.Email.ToLower() == Email.ToLower()))
            return Conflict("Admin already exists");

        string salt = _authService.GenerateSalt();
        string hashedPassword = _authService.HashPassword(Password, salt);
        string uidKey = _authService.GenerateUniqueUIDKey();
        var admin = new Admin

        {
            Email = Email,
            PasswordHash = hashedPassword,
            FullName = Fullname,
            PhoneNumber = Phonenumber,
            Salt = salt,
            Role = RoleManager.Admin,
            UID = Guid.NewGuid(),
            UIDKEY = uidKey,
            IsActive = true,
            IsSuspended = false

        };
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();


        /*var result = new AddAdminResult
       {
            AdminUID = admin.UID,
            Key = uidKey,

        };*/

        var result = new AddAdminResult
        {
            AdminUID = admin.UID.ToString(),
            Email = admin.Email,
            FullName = admin.FullName,
            Key = uidKey,
            PhoneNumber = admin.PhoneNumber,
            Role = admin.Role.ToString()
        };

        return Ok(result);

    }

[HttpPost("AddAdminH")]
[EnableCors("AllowReactApp")]
public async Task<IActionResult> AddAdminnAsyncH([FromForm] string Email, [FromForm] string Password, [FromForm] string Fullname, [FromForm] string Phonenumber )
{    
    
 if (_context.Admins.Any(p => p.Email.ToLower()== Email.ToLower()))
    return Conflict("Admin already exists");
                
 string salt = _authService.GenerateSalt();
 string hashedPassword = _authService.HashPassword(Password, salt);
 string uidKey = _authService.GenerateUniqueUIDKey();
 var admin = new AdminH

 {
     Email = Email,
     PasswordHash = hashedPassword,
     FullName = Fullname,
     PhoneNumber = Phonenumber,
     Salt = salt,
     Role = RoleManager.AdminH,
     UID = Guid.NewGuid(),
     UIDKEY = uidKey,
     IsActive = true,
     IsSuspended = false,
     Adress = "Default Address", // Replace with actual value
     PostalCode = "00000", // Replace with actual value
     DateofBirth = DateTime.Now.AddYears(-30), // Replace with actual value
     Age = 30, // Replace with actual value
     Gender = false, // Replace with actual value
     AccountStatus = false // Replace with actual value
 };
 _context.AdminHs.Add(admin);
  await _context.SaveChangesAsync();


        /*var result = new AddAdminResult
       {
            AdminUID = admin.UID,
            Key = uidKey,

        };*/

        var result = new AddAdminResult
        {
            AdminUID = admin.UID.ToString(),
            Email = admin.Email,
            FullName = admin.FullName,
            Key = uidKey,
            PhoneNumber = admin.PhoneNumber,
            Role = admin.Role.ToString()
        };

  return Ok(result); 

}


}