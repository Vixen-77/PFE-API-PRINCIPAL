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
using System.Globalization;

[ApiController]
[Route("api/ProS")]

public class FctProS : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AddAdmin> _logger;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;


    public FctProS(AuthService authService, ILogger<AddAdmin> logger, AppDbContext context, GlobalService globalService, EmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _globalService = globalService;
        _context = context;
    }



    [HttpPost("ListAlerts")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ListAlerts([FromForm] string ProSid)
    {
        var uid = Guid.Parse(ProSid);
        var pro = await _context.ProSs
        .FirstOrDefaultAsync(a => a.UID == uid);
        if (pro == null)
        {
            return NotFound("ProS not found.");
        }
        if (pro.IsAvailable == Availibility.DispoForEmergencysOnly)
        {
            var alerts = await _context.Alerts
            .Where(a => a.State == 0 && a.Color == "rouge")
            .ToListAsync();
            return Ok(alerts);
        }
        else if (pro.IsAvailable == Availibility.DispoForCallsOnly)
        {
            var alerts = await _context.Alerts
            .Where(a => a.State == 0 && a.Color == "orange")
            .ToListAsync();
            return Ok(alerts);
        }
        else
        {
            return Ok("ProS is not available for alerts.");
        }



    }


    [HttpPost("GetInfoPatMed")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> GetInfoPatientMed([FromForm] string PatientUID)
    {
        var uid = Guid.Parse(PatientUID);
        var patient = await _context.Patientss
        .FirstOrDefaultAsync(p => p.UID == uid);

        var medrec = await _context.MedRecs.Where(m => m.PatientUID == uid && m.State == MedRecState.valid).ToListAsync();

        var response = new
        {
            Patient = patient,
            MedRecs = medrec
        };

        return Ok(response);
    }

    [HttpPost("SetStatus")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> SetStatus([FromForm] string ProS, [FromForm] string state) //1appel 0urgence -1indispo
    {
        var uid = Guid.Parse(ProS);
        var pro = await _context.ProSs
        .FirstOrDefaultAsync(a => a.UID == uid);
        if (pro != null)
        {
            if (state == "1")
            {
                pro.IsAvailable = Availibility.DispoForCallsOnly;
            }
            else if (state == "0")
            {
                pro.IsAvailable = Availibility.DispoForEmergencysOnly;
            }
            else if (state == "-1")
            {
                pro.IsAvailable = Availibility.indisponible;
            }
            else
            {
                return BadRequest("Invalid state value.");
            }
            await _context.SaveChangesAsync();
            return Ok("ProS status updated successfully.");
        }
        else
        {
            return NotFound("ProS not found.");
        }
    }

    [HttpPost("AcceptAlert")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> AcceptAlert([FromForm] string ProS, [FromForm] string IdAlert, [FromForm] string lat, [FromForm] string longt) 
    {
        var uid = Guid.Parse(ProS);
        var pro = await _context.ProSs
        .FirstOrDefaultAsync(a => a.UID == uid);
        if (pro == null)
        {
            return NotFound("ProS not found.");
        }
        var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.ProSID == uid && a.State != 2);
        if (alert != null)
        {
            Console.WriteLine("ProS already taking care of an alert");
            return BadRequest("Pro S already taking care of an alert");
        }
        var alertid = Guid.Parse(IdAlert);
        var alertToAccept = await _context.Alerts
            .FirstOrDefaultAsync(a => a.AlertID == alertid);

        if (alertToAccept == null)
        {
            return NotFound("Alert not found.");
        }

        alertToAccept.ProSID = uid;
        alertToAccept.State = 1;
        pro.latitudePro = float.Parse(lat, CultureInfo.InvariantCulture);
        pro.longitudePro = float.Parse(longt, CultureInfo.InvariantCulture);

        await _context.SaveChangesAsync();

        return Ok("Alert accepted successfully.");

    }
    
    [HttpPost("FinishAlert")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> FinishAlert([FromForm] string ProS, [FromForm] string IdAlert)
    {
        var uid = Guid.Parse(ProS);
        var pro = await _context.ProSs
        .FirstOrDefaultAsync(a => a.UID == uid);
        if (pro == null)
        {
            return NotFound("ProS not found.");
        }
        var alertid = Guid.Parse(IdAlert);
        var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.AlertID == alertid);

        if (alert == null)
        {
            return NotFound("Alert not found.");
        }
        

        alert.State = 2; // Set the state to finished
        await _context.SaveChangesAsync();

        return Ok("Alert finished successfully.");

    }

}