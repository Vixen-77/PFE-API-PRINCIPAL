using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using LibrarySSMS;
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using APIAPP.Services;
using APIAPP.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

[Route("api/procheaddsupp")]
[ApiController]
public class ADDProchet : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly UploaderPatient _methodePatientService;
    private readonly AppDbContext _context;

    public ADDProchet(
        EmailService emailService,
        AppDbContext context,
        UploaderPatient methodePatientService)
    {
        _emailService = emailService;
        _context = context;
        _methodePatientService = methodePatientService;
    }

    [HttpPost("addedit")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> RajoutModifProche([FromForm] ProcheAdd request)
    {

        if (request == null)
            return BadRequest("Fichier non fourni ou vide.");

        if (request.ProcheID != null)
        {
        var PatID2=Guid.Parse(request.PatientUID);
        var send2 = await _methodePatientService.Modifproche(request.ProcheID,PatID2,request.Name,request.PhoneNumber);
        if (!send2)
            return BadRequest("Ce contact n existe pas");

        return Ok("Proche modifié avec succès.");  
        }

        var PatID=Guid.Parse(request.PatientUID);
        var send = await _methodePatientService.Addproche(request.Name,PatID,request.PhoneNumber);
        if (send == null || send == "faux")
            return BadRequest("Erreur lors de l'ajout du proche.");

        return Ok(send);   

    }

    [HttpPost("delete")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> supprimerproche([FromForm] ProcheDel request)
    {

        if (request == null)
         return BadRequest("Fichier non fourni ou vide.");

        var PatID=Guid.Parse(request.PatientUID);
        var send = await _methodePatientService.Deleteproche(PatID,request.ProcheID);
        if (!send)
            return BadRequest("Erreur lors de l'ajout du proche.");

        return Ok("Proche ajouté avec succès.");   

    }

    [HttpPost("recupListContact")]
    [EnableCors("AllowReactApp")]
        public async Task<IActionResult> RecupeContact([FromBody]string patient)
        {
        if (!Guid.TryParse(patient, out Guid patientId))
            {
            return BadRequest("ID de patient invalide.");
            }

        var Proches = await _context.Proches
            .Where(n => n.PatientUID == patientId)
            .OrderByDescending(n => n.Name)
            .Select(n => new
            {
            n.IdProche,
            n.Name,
            n.PhoneNumber,
            })
            .ToListAsync();

         if (Proches == null || !Proches.Any())
            {
            return Ok("Aucun document médical trouvé pour ce patient.");
        }

        return Ok(Proches);
    }


}