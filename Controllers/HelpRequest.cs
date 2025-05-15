using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Models;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Cors;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;
using APIAPP.DTO;

[ApiController]
[Route("api/sending")]
public class Help : ControllerBase
{
    private readonly AuthService _authService;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;
    private readonly ILogger<EditProfile> _logger;

    public Help(AuthService authService, ILogger<EditProfile> logger, GlobalService globalService, EmailService emailService, AppDbContext context)
    {
       
        _authService = authService;
        _logger = logger;
        _globalService = globalService;
        _emailService = emailService;
        _context = context;
    }

    // Modifier l'email
    [HttpPost("Help")]
    [EnableCors("AllowReactApp")]
        public async Task<IActionResult> HelpReq([FromForm] string body,[FromForm] string id, [FromForm] string role)
        {
            var idguid = Guid.Parse(id);
            Console.WriteLine("iciiiii");
            if (role == "10")
            {
                var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == idguid);
                if (patient == null)
                    return NotFound("Patient not found");
    
                var helpform = new HelpForm
                {
                    ID = Guid.NewGuid(),
                    UID = idguid,
                    Role = role,
                    Email = patient.Email,
                    Body = body,
                    CreatedAt = DateTime.Now
                };
    
                await _context.HelpForms.AddAsync(helpform);
                await _context.SaveChangesAsync();
    
                return Ok("Help request submitted successfully.");
            }
            else if (role == "20")
            {
                var pro = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == idguid);
                if (pro == null)
                    return NotFound("Professional not found");
    
                var helpform = new HelpForm
                {
                    ID = Guid.NewGuid(),
                    UID = idguid,
                    Role = role,
                    Email = pro.Email,
                    Body = body,
                    CreatedAt = DateTime.Now
                };
    
                await _context.HelpForms.AddAsync(helpform);
                await _context.SaveChangesAsync();
    
                return Ok("Help request submitted successfully.");
            }
            else
            {
                return BadRequest("Invalid role.");
            }
        }
    }