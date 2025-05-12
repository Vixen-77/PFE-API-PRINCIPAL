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
[Route("api/editprofile")]
public class EditProfilee : ControllerBase
{
    private readonly AuthService _authService;
    private readonly GlobalService _globalService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;
    private readonly ILogger<EditProfile> _logger;

    public EditProfilee(AuthService authService, ILogger<EditProfile> logger, GlobalService globalService, EmailService emailService, AppDbContext context)
    {
       
        _authService = authService;
        _logger = logger;
        _globalService = globalService;
        _emailService = emailService;
        _context = context;
    }

    // modifier le profil de l utilisateur
    [HttpPost("changeinfo")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> UpdateInfo([FromForm] EditProfile request)
    {   
        Console.WriteLine("la");
       if(request == null)
        {
            return BadRequest("Invalid request data.");
        }
        var uidd = Guid.Parse(request.ID);
        Console.WriteLine("ici");

        if(request.Role == "10"){

        var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == uidd);
        if (patient == null) return NotFound("Patient not found");

        if (request.height !=null){patient.Height = double.TryParse(request.height.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var height) ? height : 0;
        await _context.SaveChangesAsync();}

        if (request.weight !=null){patient.Weight = double.TryParse(request.weight.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var weight) ? weight : 0;
        await _context.SaveChangesAsync();}

        if (request.postalcode !=null){patient.PostalCode = request.postalcode;
        await _context.SaveChangesAsync();}

        if (request.address !=null){patient.Adresse = request.address;
        await _context.SaveChangesAsync();}

        var response1 = new
        {
            Newpostalcodepatient = patient.PostalCode,
            Newaddresspatient = patient.Adresse,
            Newheightpatient = patient.Height,
            Newweightpatient = patient.Weight,
        };
        
        return Ok(response1);

        
        }
        else {
        var proS = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == uidd);
        if (proS == null) return NotFound("Professiel de santé not found");

        if (request.postalcode !=null){proS.PostalCode = request.postalcode;
        await _context.SaveChangesAsync();}

        if (request.address !=null){proS.Adress = request.address;
        await _context.SaveChangesAsync();}
        
        var response2 = new
        {
            NewpostalcodeproS = proS.PostalCode,
            NewaddressproS = proS.Adress,
        };
        
        return Ok(response2);

    
        }
    }

    [HttpPost("ProfilePic")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> SetPFP([FromForm]ProfilePicDto request)
    {
            Console.WriteLine("icii");
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Fichier manquant.");

            if(request.Role == "10"){ 

            //  nom de fichier a base du UID du patient récup dans request.ID
            string fileName = request.ID.ToString() + Path.GetExtension(request.File.FileName);
    
           // Dossier de dest
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePicPatient"); // Dossier de destination
            if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath); // Crée le dossier s'il n'existe pas

           // 3. Chemin Absolut
            string filePath = Path.Combine(folderPath, fileName);

           // 4. Sauvegarde physique du fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
             request.File.CopyTo(stream);
            }
            await Task.Delay(1000); // Simule un délai de 1 seconde pour la sauvegarde du fichier
            //TODO: normalement on s'arrete la 

           // 5. Stockage du chemin relatif ou nom dans la BDD //:FIXME:
            string relativePath = Path.Combine("ProfilePicPatient", fileName); // à stocker dans la DB   // a voir avec melinda 
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == Guid.Parse(request.ID));
            if (patient == null) return NotFound("Patient not found");
            patient.ProfilPic= relativePath;
            await _context.SaveChangesAsync();
            filePath = "http://192.168.1.102:5001/" + relativePath.Replace("\\","/");
            
            return Ok(filePath);
            }

            if(request.Role == "20")
            {
                //  nom de fichier a base du UID du patient récup dans request.ID
            string fileName = request.ID.ToString() + Path.GetExtension(request.File.FileName);
    
           // Dossier de dest
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePicProS"); // Dossier de destination
            if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath); // Crée le dossier s'il n'existe pas

           // 3. Chemin Absolut
            string filePath = Path.Combine(folderPath, fileName);

           // 4. Sauvegarde physique du fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
             request.File.CopyTo(stream);
            }
            await Task.Delay(1000); // Simule un délai de 1 seconde pour la sauvegarde du fichier
            //TODO: normalement on s'arrete la 

           // 5. Stockage du chemin relatif ou nom dans la BDD //:FIXME:
           string relativePath = Path.Combine("ProfilePicProS", fileName); // à stocker dans la DB 
            return Ok();

            }
            return BadRequest("Invalid role specified.");
    }

    [HttpPost("DeleteProfilePic")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> DeleteProfilePic([FromForm]DeleteProfilePicRequest request)
    {
    if (string.IsNullOrEmpty(request.Uid) || string.IsNullOrEmpty(request.Role))
        return BadRequest("UID et rôle requis");

    string folder = string.Empty;

    // Vérifie si l'UID est un GUID valide
    if (!Guid.TryParse(request.Uid, out Guid guid))
        return BadRequest("UID invalide");

    if (request.Role == "10") // Patient
    {
        var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == guid);
        if (patient == null)
            return NotFound("Patient introuvable");

        if (!string.IsNullOrEmpty(patient.ProfilPic))
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), patient.ProfilPic);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            patient.ProfilPic = null; // Retire le chemin dans la DB
            _context.Patientss.Update(patient);
            await _context.SaveChangesAsync();
        }

        return Ok("Photo de profil du patient supprimée");
    }
    else if (request.Role == "20") // ProS
    {
        var pros = await _context.ProSs.FirstOrDefaultAsync(p => p.UID == guid);
        if (pros == null)
            return NotFound("Professionnel de santé introuvable");

        if (!string.IsNullOrEmpty(pros.ProfilPic))
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), pros.ProfilPic);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            pros.ProfilPic = null;
            _context.ProSs.Update(pros);
            await _context.SaveChangesAsync();
        }

        return Ok("Photo de profil du professionnel supprimée");
    }

    return BadRequest("Rôle non reconnu");
}   


}