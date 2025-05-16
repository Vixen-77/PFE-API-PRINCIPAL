using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LibrarySSMS.Models;
using APIAPP.Services;
using APIAPP.DTO;
using LibrarySSMS.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibrarySSMS;


namespace APIAPP.Controllers {

[ApiController]
[Route("api/auth")]
public class PatientRecup : ControllerBase {
    
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;

        public PatientRecup(
            EmailService emailService,
            AppDbContext context)
        {
           
           
            _emailService = emailService;
            _context = context;
            ;
        }

[HttpPost("PatientBecomeValidated")]
[EnableCors("AllowReactApp")]
public IActionResult GetPatientValidate([FromBody] ValidationRequest request)
{
    var patient = _context.Patientss.FirstOrDefault(p => p.UID == request.PatientUid);
    if (patient == null)
        return NotFound("Patient introuvable.");

    // Mise à jour
    //FIXME:AAAAA patient.IsValidated = request.validation;
    _context.SaveChanges();

    var good =  _emailService.SendEmailAsyncValidation(patient.Email, "Validation de votre compte", "Votre compte a été validé avec succès vous pouvez acceder a notre platforme .");
    return Ok("Mise à jour effectuée avec succès.");
}

















}

}
