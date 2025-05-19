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
[Route("api/Admin")]

public class FctAdmin : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AddAdmin> _logger;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;

    public FctAdmin(AuthService authService, ILogger<AddAdmin> logger, AppDbContext context,GlobalService globalService,EmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _globalService = globalService;
        _context = context;
    }

    //verification patient

    [HttpPost("ListVerifyPatient")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ListPatients([FromForm] string IDAdminn, [FromForm] int roleadm)
    {

        var IDAdmin = Guid.Parse(IDAdminn);
        object? admin = null;

        if (roleadm == 0) //si 0 : admin
        {
            admin = await _context.Admins.FirstOrDefaultAsync(p => p.UID == IDAdmin);
        }
        else //si 1 : super admin
        {
            admin = await _context.SuperAdmins.FirstOrDefaultAsync(p => p.UID == IDAdmin);
        }
        if (admin == null)
        {
            return NotFound("Admin not found");
        }
        else
        {
            Guid adminUID = roleadm == 0
                ? ((Admin)admin).UID
                : ((SuperAdmin)admin).UID;

            //on charge sa liste de patients a confirmer
            var listepatients = await _context.CreationCompte
                .Where(c => c.AdminUID == adminUID && c.State == 0 && c.Role == "10")
                .Select(c => new { c.Id, c.UserUID })
                .ToListAsync();

            return Ok(listepatients);
        }
    }



    [HttpPost("ListVerifyProS")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ListProS([FromForm] string IDAdminn, [FromForm] int roleadm)
    {
        var IDAdmin = Guid.Parse(IDAdminn);
        object? admin = null;

        if (roleadm == 0) //si 0 : admin
        {
            admin = await _context.Admins.FirstOrDefaultAsync(p => p.UID == IDAdmin);
        }
        else //si 1 : super admin
        {
            admin = await _context.SuperAdmins.FirstOrDefaultAsync(p => p.UID == IDAdmin);
        }
        if (admin == null)
        {
            return NotFound("Admin not found");
        }
        else
        {
            Guid adminUID = roleadm == 0
                ? ((Admin)admin).UID
                : ((SuperAdmin)admin).UID;

            //on charge sa liste de proS a confirmer
            var listepatients = await _context.CreationCompte
                .Where(c => c.AdminUID == adminUID && c.State == 0 && c.Role == "20")
                .Select(c => new { c.Id, c.UserUID })
                .ToListAsync();

            return Ok(listepatients);
        }
    }

    [HttpPost("GetInfoPatient")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> GetInfoPatient([FromForm] string IDPatientt)
    {
        var IDPatient = Guid.Parse(IDPatientt);
        var patient = await _context.Patientss
                    .Where(p => p.UID == IDPatient)
                    .Select(p => new
                    {
                        p.UID,
                        p.Name,
                        p.LastName,
                        p.Email,
                        p.PhoneNumber,
                        p.Gender,
                        p.DateofBirth,
                        p.Age,
                        p.Adresse,
                        p.PostalCode,
                        p.ConfMail,
                        p.identite,
                        p.Height,
                        p.Weight
                    })
                .FirstOrDefaultAsync();

        if (patient == null)
        {
            return NotFound("Patient not found");
        }
        else
        {
            return Ok(patient);
        }

    }

    [HttpPost("GetInfoProS")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> GetInfoProS([FromForm] string IDProSs)
    {
        var IDProS = Guid.Parse(IDProSs);
        var proS = await _context.ProSs
                    .Where(p => p.UID == IDProS)
                    .Select(p => new
                    {
                        p.UID,
                        p.Name,
                        p.LastName,
                        p.Email,
                        p.PhoneNumber,
                        p.Gender,
                        p.DateofBirth,
                        p.Age,
                        p.Adress,
                        p.PostalCode,
                        p.ConfMail,
                        p.identite,
                        p.Certif
                    })
                .FirstOrDefaultAsync();

        if (proS == null)
        {
            return NotFound("Patient not found");
        }
        else
        {
            return Ok(proS);
        }

    }

    [HttpPost("Validate")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ValidateUser([FromForm] string idCreaCompte, [FromForm] string role)
    {
        var creacid = Guid.Parse(idCreaCompte);
        var crareq = await _context.CreationCompte.FirstOrDefaultAsync(c => c.Id == creacid);
        if (crareq == null)
        {
            return NotFound("Request not found.");
        }
        else if (crareq.State == 1)
        {
            return Ok("Request already received treatment.");
        }
        else
        {
            if (role == "10") //patient
            {
                var uid = crareq.UserUID;
                var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
                if (patient == null)
                {
                    return NotFound("Patient not found.");
                }
                else
                {
                    crareq.State = 1;
                    patient.IsValidated = 1;
                    await _context.SaveChangesAsync();
                    return Ok("Patient validated with succes.");
                }
            }
            else if (role == "20") //proS
            {
                var uid = crareq.UserUID;
                var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
                if (proS == null)
                {
                    return NotFound("Healthcare Pro not found.");
                }
                else
                {
                    crareq.State = 1;
                    proS.IsValidated = 1;
                    await _context.SaveChangesAsync();
                    return Ok("Healthcare Pro validated with succes.");
                }
            }
            else
            {
                return BadRequest("Role undefined");
            }
        }
    }

    [HttpPost("Reject")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> RejectUser([FromForm] string idCreaCompte, [FromForm] string role)
    {
        var creacid = Guid.Parse(idCreaCompte);
        var crareq = await _context.CreationCompte.FirstOrDefaultAsync(c => c.Id == creacid);
        if (crareq == null)
        {
            return NotFound("Request not found.");
        }
        else if (crareq.State == 1)
        {
            return Ok("Request already received treatment.");
        }
        else
        {
            if (role == "10") //patient
            {
                var uid = crareq.UserUID;
                var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
                if (patient == null)
                {
                    return NotFound("Patient not found.");
                }
                else
                {
                    crareq.State = 1;
                    patient.IsValidated = -1;
                    await _context.SaveChangesAsync();
                    return Ok("Patient rejected with succes.");
                }
            }
            else if (role == "20") //proS
            {
                var uid = crareq.UserUID;
                var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
                if (proS == null)
                {
                    return NotFound("Healthcare Pro not found.");
                }
                else
                {
                    crareq.State = 1;
                    proS.IsValidated = -1;
                    await _context.SaveChangesAsync();
                    return Ok("Healthcare Pro rejected with succes.");
                }
            }
            else
            {
                return BadRequest("Role undefined");
            }
        }
    }


    [HttpPost("ListActiveAlerts")]
    [EnableCors("AllowReactApp")]

    public async Task<IActionResult> ListActiveAlerts()
    {
        var ListActiveAlerts = await _context.Alerts
                                .Where(a => a.State == 0 || a.State == 1) //pas traite ou en cours de traitement
                                .Select(a => a.PatientUID)
                                .ToListAsync();
        return Ok(ListActiveAlerts);
    }

    [HttpPost("ListFinishedAlerts")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ListFinishedAlerts()
    {
        var ListFinishedAlerts = await _context.Alerts
                                .Where(a => a.State == 2) //finis
                                .Select(a => a.PatientUID)
                                .ToListAsync();

        return Ok(ListFinishedAlerts);
    }

    [HttpPost("GetInfoAlert")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> GetInfoAlert([FromForm] string idpat)
    {
        var UID = Guid.Parse(idpat);
        var Alerte = await _context.Alerts.FirstOrDefaultAsync(a => a.PatientUID == UID);
        if (Alerte == null)
        {
            return NotFound("Alert not found.");
        }
        else 
        {
            //on retrouve le patient pr avoir ses info
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == UID);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            //on retrouve pro s qui prend en charge (eventuellement)
            var proS = await _context.ProSs.FirstOrDefaultAsync(s => s.UID == Alerte.ProSID);

            var InfoAlert = new InfoAlert()
            {
                State = Alerte.State,
                PatientID = Alerte.PatientUID.ToString(),
                Email = patient.Email,
                Name = patient.Name,
                LastName = patient.LastName,
                height = ((float)patient.Height).ToString(),
                weight = ((float)patient.Weight).ToString(),
                phonenumber = patient.PhoneNumber,
                postalcode = patient.PostalCode,
                address = patient.Adresse,
                birthdate = patient.DateofBirth.ToString(),
                latitudePatient = Alerte.latitudePatient.ToString() ?? "",
                longitudePatient = Alerte.longitudePatient.ToString() ?? "",
                Location = Alerte.Location,
                Gender = patient.Gender == true ? "male" : "female",
                ProSID = proS?.UID.ToString() ?? "",
                firstnamepro = proS?.Name ?? "",
                lastnamepro = proS?.LastName ?? "",
                emailProS = proS?.Email ?? "",
                phonenumberProS = proS?.PhoneNumber ?? "",
                latitudeProS = proS?.latitudePro.ToString() ?? "",
                longitudeProS = proS?.longitudePro.ToString() ?? "",
                Color = Alerte.Color,
                CreatedAt = Alerte.CreatedAt,
            };
            return Ok(InfoAlert);
        }
    }

    [HttpPost("RechercheMail")]
    [EnableCors("AllowReactApp")]       //recherce a partir de l id ou mail (pour la page de moderation)
    public async Task<IActionResult> SearchUser([FromForm] string? id, [FromForm] string? email, [FromForm] string role)
    {
        if (role == "10")
        { //on cherche un patient

            if (id != null)
            {
                var uid = Guid.Parse(id);
                var patient = await _context.Patientss.FirstOrDefaultAsync(u => u.UID == uid);
                return Ok(patient);
            }
            else
            {
                var patient = await _context.Patientss.FirstOrDefaultAsync(u => u.Email == email);
                return Ok(patient);
            }
        }
        else if (role == "20")
        { //on cherche un proS
            if (id != null)
            {
                var uid = Guid.Parse(id);
                var proS = await _context.ProSs.FirstOrDefaultAsync(u => u.UID == uid);
                return Ok(proS);
            }
            else
            {
                var proS = await _context.ProSs.FirstOrDefaultAsync(u => u.Email == email);
                return Ok(proS);
            }

        }
        else
        {
            return BadRequest("Bad Request");
        }
    }

    [HttpPost("AllPatients")]
    [EnableCors("AllowReactApp")] //liste de tt les patients
    public async Task<IActionResult> AllPatients()
    {
        var ListPat = await _context.Patientss.ToListAsync();
        return Ok(ListPat);

    }

    [HttpPost("AllProS")]
    [EnableCors("AllowReactApp")] //liste de tt les proS
    public async Task<IActionResult> AllProS()
    {
        var ListProS = await _context.ProSs.ToListAsync();
        return Ok(ListProS);

    }

    [HttpPost("BanUser")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> BanUser([FromForm] string id, [FromForm] string role)
    {
        var uid = Guid.Parse(id);
        if (role == "10")
        {
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patient.IsBanned = true;
            await _context.SaveChangesAsync();
            return Ok("Patient banned with succes");
        }
        else if (role == "20")
        {
            var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
            if (proS == null)
            {
                return NotFound("Healthcare Pro not found");
            }
            proS.IsBanned = true;
            await _context.SaveChangesAsync();
            return Ok("Healthcare Pro banned with succes");
        }
        else
        {
            return BadRequest("Bad request");
        }
    }

    [HttpPost("SuspendUser")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> Suspend([FromForm] string id, [FromForm] string role)
    {
        var uid = Guid.Parse(id);
        if (role == "10")
        {
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patient.AccountStatus = true;
            await _context.SaveChangesAsync();
            return Ok("Patient suspended with succes");
        }
        else if (role == "20")
        {
            var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
            if (proS == null)
            {
                return NotFound("Healthcare Pro not found");
            }
            proS.AccountStatus = true;
            await _context.SaveChangesAsync();
            return Ok("Healthcare Pro suspended with succes");
        }
        else
        {
            return BadRequest("Bad request");
        }
    }

    [HttpPost("RemoveSuspension")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> RemoveSus([FromForm] string id, [FromForm] string role)
    {
        var uid = Guid.Parse(id);
        if (role == "10")
        {
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patient.AccountStatus = false;
            await _context.SaveChangesAsync();
            return Ok("Patient in-suspended with succes");
        }
        else if (role == "20")
        {
            var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
            if (proS == null)
            {
                return NotFound("Healthcare Pro not found");
            }
            proS.AccountStatus = false;
            await _context.SaveChangesAsync();
            return Ok("Healthcare Pro in-suspended with succes");
        }
        else
        {
            return BadRequest("Bad request");
        }
    }

    [HttpPost("ListHelpForms")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> ListHelpForms()
    {
        var ListHelpForms = await _context.HelpForms
                                .ToListAsync();

        return Ok(ListHelpForms);
    }




    [HttpPost("HelpFormResponse")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> HelpResp([FromForm] string id,[FromForm]string response)
    {   
        string? mail = null;
        var idF = Guid.Parse(id);
        var help = await _context.HelpForms.FirstOrDefaultAsync(h => h.ID == idF);
        if (help == null)
        {
            return Ok("This Help Request has already been taken care of");
        }
        else
        {
            if (help.Role == "10")
            { //on cherche le mail du patient pr lui rep
                var uid = help.UID;
                var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uid);
                if (patient == null)
                {
                    return BadRequest("Patient doesnt exist");
                }
                else
                {
                     mail = patient.Email;
                }
            }
            else if (help.Role == "20")
            { //on cherche le mail du proS pr lui rep
                var uid = help.UID;
                var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uid);
                if (proS == null)
                {
                    return BadRequest("Patient doesnt exist");
                }
                else
                {
                     mail = proS.Email;
                }
            }
            else
            {
                return BadRequest("Bad Request");
            }
            //on env le mail
            string head ="Response to Help Form";
            string body = $@"
            <h3>You asked:</h3><br><p>{help.Body}</p><br><br>
            <h3>Our response:<h3>
            <br>
            {response}
            ";
            var good = await _emailService.SendEmailAsyncValidation(mail, head, body);
            if (!good)
            {
                return BadRequest("Error while sending mail");
            }

            _context.HelpForms.Remove(help);
            await _context.SaveChangesAsync();
            return Ok("Response sent succesfully");
        }
    }

}