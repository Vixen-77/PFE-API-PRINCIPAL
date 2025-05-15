using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.DTO;
using LibrarySSMS.Models;
using APIAPP.Services;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestSharp;
using Microsoft.EntityFrameworkCore;

namespace APIAPP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class Logout : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<PatientController> _logger;
        private readonly AppDbContext _context;

        public Logout(
            AuthService authService,
            ILogger<PatientController> logger,
            AppDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        [HttpPost("Logout")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> Logoutt([FromForm] LogoutPatientRequest request)
        {
            var idguid = Guid.Parse(request.Id);
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }
             if (request.Role == "10"){
             var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == idguid);
             if (patient == null)
             {
                return NotFound("Patient not found");
             }
             patient.IsActive = false;
             //revoke le token ddu coup:
             await _context.SaveChangesAsync();
             return Ok( "Déconnexion réussie.");
            } 


            else
             {if(request.Role == "20"){
             var pro = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == idguid);
             if (pro == null)
             {
                return NotFound("Patient not found");
             }
             pro.IsActive = false;
             //revoke le token ddu coup:
             await _context.SaveChangesAsync();
             return Ok("Déconnexion réussie.");
            }

            }
            return BadRequest("Invalid request data.");          
        }
    }}
