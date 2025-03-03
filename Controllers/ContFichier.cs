using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using APIAPP.Models;
using APIAPP.Data;

[Route("api/patients")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PatientController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadMedicalRecord([FromForm] IFormFile file, [FromForm] int patientId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Fichier invalide.");
        }

        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound("Patient non trouvé.");
        }

        // 📂 Définir le chemin de stockage
        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadFolder = Path.Combine(webRootPath, "uploads", "medical_records");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadFolder, fileName);

        // 📥 Sauvegarde du fichier sur le serveur
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 📌 Mise à jour du patient avec le chemin du fichier
        patient.MedicalRecordPath = $"/uploads/medical_records/{fileName}";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Fichier uploadé avec succès", filePath = patient.MedicalRecordPath });
    }
}
