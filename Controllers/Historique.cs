using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Services;
using LibrarySSMS.Models;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Cors;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/historiquealert")]
public class AlertHisto : ControllerBase
{
    private readonly AuthService _authService;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;
    private readonly ILogger<AlertHisto> _logger;

    public AlertHisto(AuthService authService, ILogger<AlertHisto> logger, GlobalService globalService, EmailService emailService, AppDbContext context)
    {
        _authService = authService;
        _logger = logger;
        _globalService = globalService;
        _emailService = emailService;
        _context = context;
    }

    [HttpPost("createAlt")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> CreateAlert([FromForm] string Pid, [FromForm] string Proid)
    {
        if (string.IsNullOrWhiteSpace(Pid) || string.IsNullOrWhiteSpace(Proid))
            return BadRequest("Missing Pid or Proid");

        if (!Guid.TryParse(Pid, out Guid patientId))
            return BadRequest("Invalid PatientUID");

        if (!Guid.TryParse(Proid, out Guid proId))
            return BadRequest("Invalid ProSID");

        var alert = new Alert
        {
            AlertID = Guid.NewGuid(),
            PatientUID = patientId,
            ProSID = proId,
            CreatedAt = DateTime.UtcNow,
            Color = "Red",
            State = 0,
            Descrip = "Hypoglycémie",
            IsRead = false,
            latitudePatient = "36.72615",
            longitudePatient = "3.08647",
        };

        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        return Ok(alert);
    }

    [HttpPost("historique")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> Listealert([FromForm] string Role, [FromForm] string ID)
    {
        if (string.IsNullOrWhiteSpace(Role) || string.IsNullOrWhiteSpace(ID))
            return BadRequest("Missing Role or ID");

        if (!Guid.TryParse(ID, out Guid userId))
            return BadRequest("Invalid ID format");

        if (Role == "10") 
        {
            var ownAlertsRaw = await _context.Alerts
                .Where(a => a.PatientUID == userId)
                .ToListAsync();

            var ownAlerts = ownAlertsRaw.Select(a => new
            {
                alertID = a.AlertID.ToString(),
                id = a.PatientUID.ToString(),
                descrip = a.Descrip,
                createdAt = a.CreatedAt,
                color = a.Color,
                name = (string?)null,
                lastName = (string?)null,
                latitude = TryParseCoordinate(a.latitudePatient),
                longitude = TryParseCoordinate(a.longitudePatient)
            })
            .OrderByDescending(a => a.createdAt)
            .ToList();

            var otherRedAlertsRaw = await _context.Alerts
                .Where(a => a.Color == "rouge" && a.PatientUID != userId)
                .Join(_context.Patientss,
                      alert => alert.PatientUID,
                      patient => patient.UID,
                      (alert, patient) => new
                      {
                          alert.AlertID,
                          alert.Descrip,
                          alert.CreatedAt,
                          alert.Color,
                          patient.Name,
                          patient.LastName,
                          alert.latitudePatient,
                          alert.longitudePatient,
                          alert.PatientUID
                      })
                .ToListAsync();

            var otherRedAlerts = otherRedAlertsRaw.Select(a => new
            {
                alertID = a.AlertID.ToString(),
                id = a.PatientUID.ToString(),
                descrip = a.Descrip,
                createdAt = a.CreatedAt,
                color = a.Color,
                name = a.Name,
                lastName = a.LastName,
                latitude = TryParseCoordinate(a.latitudePatient),
                longitude = TryParseCoordinate(a.longitudePatient)
            })
            .OrderByDescending(a => a.createdAt)
            .ToList();

            if (!ownAlerts.Any() && !otherRedAlerts.Any())
                return NotFound("No alerts found");

            return Ok(new
            {
                ownAlerts,
                otherAlerts = otherRedAlerts
            });
        }
        else if (Role == "20")
        {
            
            var takenAlertsRaw = await _context.Alerts
                .Where(a => a.ProSID == userId)
                .Join(_context.Patientss,
                      alert => alert.PatientUID,
                      patient => patient.UID,
                      (alert, patient) => new
                      {
                          alert.AlertID,
                          alert.Descrip,
                          alert.CreatedAt,
                          alert.Color,
                          patient.Name,
                          patient.LastName,
                          alert.latitudePatient,
                          alert.longitudePatient,
                          alert.PatientUID
                      })
                .ToListAsync();

            var takenAlerts = takenAlertsRaw.Select(a => new
            {
                alertID = a.AlertID.ToString(),
                id = a.PatientUID.ToString(),
                descrip = a.Descrip,
                createdAt = a.CreatedAt,
                color = a.Color,
                name = a.Name,
                lastName = a.LastName,
                latitude = TryParseCoordinate(a.latitudePatient),
                longitude = TryParseCoordinate(a.longitudePatient)
            })
            .OrderByDescending(a => a.createdAt)
            .ToList();

            
            var otherAlertsRaw = await _context.Alerts
                .Where(a => a.Color == "rouge" && (a.ProSID == null || a.ProSID != userId))
                .Join(_context.Patientss,
                      alert => alert.PatientUID,
                      patient => patient.UID,
                      (alert, patient) => new
                      {
                          alert.AlertID,
                          alert.Descrip,
                          alert.CreatedAt,
                          alert.Color,
                          patient.Name,
                          patient.LastName,
                          alert.latitudePatient,
                          alert.longitudePatient,
                          alert.PatientUID
                      })
                .ToListAsync();

            var otherAlerts = otherAlertsRaw.Select(a => new
            {
                alertID = a.AlertID.ToString(),
                id = a.PatientUID.ToString(),
                descrip = a.Descrip,
                createdAt = a.CreatedAt,
                color = a.Color,
                name = a.Name,
                lastName = a.LastName,
                latitude = TryParseCoordinate(a.latitudePatient),
                longitude = TryParseCoordinate(a.longitudePatient)
            })
            .OrderByDescending(a => a.createdAt)
            .ToList();

            return Ok(new
            {
                takenAlerts,
                otherAlerts
            });
        }

        return NotFound("Role not found");
    }

    private double? TryParseCoordinate(string? coordinate)
    {
        if (string.IsNullOrWhiteSpace(coordinate))
            return null;

        coordinate = coordinate.Replace(',', '.'); 

        return double.TryParse(
            coordinate,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var result)
            ? result
            : null;
    }
}



