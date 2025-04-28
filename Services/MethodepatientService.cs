using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using LibrarySSMS.Models;
using LibrarySSMS;
using LibrarySSMS.Enums;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using APIAPP.DTO;

namespace APIAPP.Services
{
    public class UploaderPatient
    {
    
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly ILogger<UploaderPatient> _logger;
        private readonly AuthService _authService;

        public UploaderPatient(
            IEmailService emailService,
            AuthService authService,
            AppDbContext dbContext,
            ILogger<UploaderPatient> logger)
        {
            _emailService = emailService;
            _context = dbContext;
            _logger = logger;
            _authService = authService;
        }


public async Task<pathandID?> UploadandEmail(IFormFile file, Guid patientUid, string mailMed, string description, string title)
{
    if (file == null || file.Length == 0)
    {
        _logger.LogWarning("Fichier non fourni ou vide.");
        return null;
    }

    // 1. Création d'un nom de fichier unique
    var nouveauID = Guid.NewGuid(); 
    string fileName = nouveauID.ToString() + Path.GetExtension(file.FileName);

    // 2. Dossier de destination
    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath); // Crée le dossier s'il n'existe pas

    // 3. Chemin complet pour sauvegarde
    string filePathMed = Path.Combine(folderPath, fileName);

    // 4. Sauvegarde physique du fichier
    using (var stream = new FileStream(filePathMed, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // 5. Stockage du chemin relatif ou nom dans la BDD
    string relativePathmed = Path.Combine("Data", fileName); // à stocker dans la DB
    var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == patientUid);
    if (patient == null)
    {
        _logger.LogWarning($"Patient avec UID {patientUid} non trouvé.");
        return null; // Retourner faux si le patient n'est pas trouvé
    }

    // 6. Création de l'enregistrement médical
    MedRec newMedRec = new MedRec
    {
        UIDMedRec = Guid.NewGuid(),
        FilePath = relativePathmed,
        State = MedRecState.Pending, // Exemple de statut
        CreatedAt = DateTime.UtcNow,
        MailMed = mailMed,
        PatientUID = patientUid,
        Patient = patient,
        Title = title,
        Description = description,
    };

    // 7. Enregistrer dans la base de données
    _context.MedRecs.Add(newMedRec);
    await _context.SaveChangesAsync();

    return new pathandID{
       ID = newMedRec.UIDMedRec,
       path = relativePathmed
    };
}
    
    public async Task<string> Addproche(string name, Guid patientUid, string PhoneNumber)
    {
        var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == patientUid);
        if (patient == null)
        {
            _logger.LogWarning($"Patient avec UID {patientUid} non trouvé.");
            return "false"; // Retourner faux si le patient n'est pas trouvé
        }

        var proche = new Proche
        {
            IdProche = "Proche" + Guid.NewGuid(), //FIXME: a voir avec melinda
            Name = name,
            PhoneNumber = PhoneNumber,
            PatientUID = patientUid,
            Patient = patient,
        };

        _context.Proches.Add(proche);
        await _context.SaveChangesAsync();

        return proche.IdProche;

    }
    
    
    public async Task<bool> Deleteproche( Guid patientUid, string procheUid)
    {
        var proche = await _context.Proches.FirstOrDefaultAsync(p => p.PatientUID == patientUid && p.IdProche == procheUid);
        if (proche == null)
        {
            _logger.LogWarning($"vous n'avez pas d'amis miskin");
            return false; // Retourner faux si le patient n'est pas trouvé
        }

        _context.Proches.Remove(proche);
        await _context.SaveChangesAsync();
        return true;
    
    }

    public async Task<bool> Modifproche(string procheUid, Guid idpatient ,string name, string PhoneNumber)
    {
        var proche = await _context.Proches.FirstOrDefaultAsync(p => p.PatientUID == idpatient && p.IdProche == procheUid); 

        if (proche == null)
        {
            _logger.LogWarning($"vous n'avez pas d'amis miskin");
            return false; // Retourner faux si le patient n'est pas trouvé
        }

        proche.Name = name;
        proche.PhoneNumber = PhoneNumber;
       
        _context.Proches.Update(proche);
        await _context.SaveChangesAsync();
        return true;
    }
    
    
    }}