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
using Newtonsoft.Json.Schema;
using System.Net.Mail;
using System.Security.Cryptography;

[ApiController]
[Route("api/Alerte")]

public class Alerte : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AddAdmin> _logger;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;

    public Alerte(AuthService authService, ILogger<AddAdmin> logger, AppDbContext context, GlobalService globalService, EmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _globalService = globalService;
        _context = context;
    }

    [HttpPost("AddAlerte")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> AddAlerte([FromForm] Alerteprov request)
    {
        var uid = Guid.Parse(request.PatientUID);
        var alerte = new Alert
        {   
            AlertID = Guid.NewGuid(),
            PatientUID = uid,
            Color = request.Color,
            latitudePatient = request.latitudePatient,
            State = 0,
            longitudePatient = request.longitudePatient,
            CreatedAt = DateTime.UtcNow,
        };
        _context.Alerts.Add(alerte);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("supp")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> supp()
    {
        var all = await _context.Alerts.ToListAsync();
        _context.Alerts.RemoveRange(all);
        await _context.SaveChangesAsync();
        return Ok();
    }


}
