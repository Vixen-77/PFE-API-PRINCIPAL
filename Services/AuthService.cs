//FIXME: This service implemente
//TODO:1-Sign in,
//TODO:2-Sign up , 
//TODO:3-Logout 
//TODO:4-desactivated account 
//TODO:5-JWT call


/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////TODO:FINALISé  NE LE TOUCHE PAS RACIME SAUF AVEC MON ACCORD TODO://///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://///////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




using LibrarySSMS;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Collections.Generic;
using LibrarySSMS.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using APIAPP.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Antiforgery;
using Azure.Core;
using LibrarySSMS.Models;
using System.Linq;
using System.Threading.Tasks;
using APIAPP.DTOResponse;

namespace APIAPP.Services
{
    public class AuthService
    {   //FIXME: jwt a un probleme a reguler le plus vite possible 
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;
        private readonly HashSet<string> _revokedTokens = new HashSet<string>();

        public AuthService(AppDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        } 


        private string jwttoken = string.Empty;
       
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////FIXME: Connexion - Gère les rôles séparément FIXME://////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<SignInResultt?> SignInPatient(string email, string password)
      {
    
           var patient = _context.Patientss.FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
         if (patient == null || !VerifyPassword(password, patient.PasswordHash, patient.Salt))
         {
          throw new AuthException("Email ou mot de passe incorrect.", 401);
         }
         if (!patient.IsValidated)
         {
        throw new AuthException("Votre compte n'est pas encore validé.", 403);
         }
         // 4. Génération du token JWT
        var token = _jwtService.GenerateTokenPatient(patient);
        var maskedMail =MasquerEmail(patient.Email);
        await Task.Delay(0);
            return new SignInResultt
           {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(365), // ou selon ta logique de durée
            UID = patient.UID,
            Role = 10,
            Email = maskedMail,
            Name = patient.Name,
            LastName = patient.LastName
           };
     }
     



     public async Task<SignInResultt?> SignInProS(string email, string password)
      {
    
         var proS = _context.ProSs.FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
         if (proS == null || !VerifyPassword(password, proS.PasswordHash, proS.Salt))
         {
          throw new AuthException("Email ou mot de passe incorrect.", 401);
         }
         if (!proS.IsValidated)
         {
        throw new AuthException("Votre compte n'est pas encore validé.", 403);
         }
         // 4. Génération du token JWT
        var token = _jwtService.GenerateTokenProS(proS);
        var maskedMail =MasquerEmail(proS.Email);
        await Task.Delay(0);
        
            return new SignInResultt
           {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // ou selon ta logique de durée
            UID = proS.UID,
            Role = 20,
            Email = maskedMail,
            Name = proS.Name,
            LastName = proS.LastName
           };
     }

     public async Task <SignInResultAdmin?> SignInAdminH(string email, string password, string key)
      {
    
         var adminh = _context.AdminHs.FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.PasswordHash == password && p.UIDKEY == key);
         if (adminh == null || !VerifyPassword(password, adminh.PasswordHash, adminh.Salt))
         {
          throw new AuthException("Email ou mot de passe incorrect.", 401);
         }
        
         // 4. Génération du token JWT
        var token = _jwtService.GenerateTokenAdminH(adminh); 
        var maskedMail =MasquerEmail(adminh.Email);
        await Task.Delay(0);
        return new SignInResultAdmin
        {
        Token = token,
        ExpiresAt = DateTime.UtcNow.AddHours(2), // ou selon ta logique de durée
        UID = adminh.UID,
        Role = 30,
        Email = maskedMail,
        FullName = adminh.FullName
        };
     }
     


     public async Task<SignInResultAdmin?> SignInAdmin(string email, string password, string key)
      {
    
        var admin = _context.Admins.FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.PasswordHash == password && p.UIDKEY == key);

         if (admin== null || !VerifyPassword(password, admin.PasswordHash, admin.Salt))
         {
          throw new AuthException("Email ou mot de passe incorrect.", 401);
         }
        
         // 4. Génération du token JWT
        var token = _jwtService.GenerateTokenAdmin(admin); 
        var maskedMail =MasquerEmail(admin.Email);
        await Task.Delay(0);
        return new SignInResultAdmin
        {
        Token = token,
        ExpiresAt = DateTime.UtcNow.AddHours(2), // ou selon ta logique de durée
        UID = admin.UID,
        Role = 40,
        Email = maskedMail,
        FullName = admin.FullName
        };
     }

     public async Task<SignInResultAdmin?> SignInSuperAdmin(string email, string password, String key)
      {
    
         var superadmin = _context.SuperAdmins.FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.PasswordHash == password && p.UIDKEY == key);
         if (superadmin == null || !VerifyPassword(password, superadmin.PasswordHash, superadmin.Salt))
         {
          throw new AuthException("Email ou mot de passe incorrect.", 401);
         }
        
         // 4. Génération du token JWT
        var token = _jwtService.GenerateTokenSuperAdmin(superadmin); 
        var maskedMail =MasquerEmail(superadmin.Email);
        await Task.Delay(0);
        return new SignInResultAdmin
        {
        Token = token,
        ExpiresAt = DateTime.UtcNow.AddHours(2), // ou selon ta logique de durée
        UID = superadmin.UID,
        Role = 50,
        Email = maskedMail,
        FullName = superadmin.FullName
        };
     }






/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////FIXME: Inscription - Gère les rôles séparément FIXME://////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public async Task<SignUpResult> SignUpPatient(SignUpPatientRequest request) // FIXME:
        {
            if (_context.Patientss.Any(p => p.Email.ToLower()== request.Email.ToLower()))
                return new SignUpResult { PatientUID = null, filename = null };

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);


            // 1. Création d'un nom de fichier unique
           string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
    
           // 2. Dossier de destination 
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Datapatientidf");
            if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath); // Crée le dossier s'il n'existe pas

           // 3. Chemin complet pour sauvegarde
            string filePath = Path.Combine(folderPath, fileName);

           // 4. Sauvegarde physique du fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
             request.File.CopyTo(stream);
            }

           // 5. Stockage du chemin relatif ou nom dans la BDD
           string relativePath = Path.Combine("Datapatientidf", fileName); // à stocker dans la DB
           var newPatient = new Patient
           
            {   
                UID= Guid.NewGuid(),
                Name = request.Name, //1
                LastName = request.LastName, //1
                Email = request.Email, //2
                PasswordHash = hashedPassword,//9
                Adresse = request.Adress, //2
                Salt = salt,
                Role = request.Role,//3
                IsActive = false,
                PostalCode = request.PostalCode, //5
                DateofBirth = request.DateOfBirth,//6
                PhoneNumber = request.PhoneNumber,//7
                CreatedAt = DateTime.UtcNow,
                LastLogin = null,
                AccountStatus = false,
                TwoFactorEnabled = false,
                SubscriptionPlan = false,
                IsOnline = false,
                State= UserState.Conducteur, //par defaut mais le user peut le changé ou bien changement par automatisation
                IsValidated = false,
                IdphoneP = null,
                IdSmartwatchP =null,
                IdSmartwatchNewGenP=null,
                IdVehiculeOBUP=null,
                IdCGMP=null,
                identite = relativePath,
                Age =request.Age,
                Gender =request.Gender,  //0 si femme et 1 si homme
                Weight = request.Weight, 
                Height = request.Height,
            };

            _context.Patientss.Add(newPatient);
            await _context.SaveChangesAsync();
            return new SignUpResult
            {
                PatientUID = newPatient.UID, // Renvoie l'UID du patient inscrit
                filename = fileName // Renvoie le nom du fichier sauvegardé
            };
        }

        public async Task<SignUpResultProS> SignUpProSante(SignUpProSRequest request)
        {
            if (_context.ProSs.Any(p => p.Email == request.Email))
                return new SignUpResultProS { ProSUID = null, filename = null, filename2 = null };;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);

            // 1. Création de noms de fichiers uniques
           string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
           string fileNameCertif = Guid.NewGuid().ToString() + Path.GetExtension(request.FileCertif.FileName);

           // 2. Définir les dossiers de destination
           string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "DataProSidf");
           string folderPathCertif = Path.Combine(Directory.GetCurrentDirectory(), "DataProSCertif");

           // 3. Créer les dossiers s’ils n’existent pas
           if (!Directory.Exists(folderPath))
           Directory.CreateDirectory(folderPath);

           if (!Directory.Exists(folderPathCertif))
           Directory.CreateDirectory(folderPathCertif);

           // 4. Chemins complets pour sauvegarde
           string filePath = Path.Combine(folderPath, fileName);
           string filePathCertif = Path.Combine(folderPathCertif, fileNameCertif);

           // 5. Sauvegarde physique des fichiers
           using (var stream = new FileStream(filePath, FileMode.Create))
          {
          request.File.CopyTo(stream);
          }

           using (var stream = new FileStream(filePathCertif, FileMode.Create))
          {
          request.FileCertif.CopyTo(stream);
          }
 
          // 6. Chemins relatifs à stocker dans la DB
       string relativePath = Path.Combine("DataProSidf", fileName);
       string relativePathCertif = Path.Combine("DataProSCertif", fileNameCertif);


            var newPro = new ProS

            {
              UID =Guid.NewGuid(),
              Name = request.Name, //1
              LastName = request.LastName,
              Email = request.Email,//2
              PasswordHash = hashedPassword,//3
              Salt = salt,
              Role = request.Role, // Conversion explicite//4
              Age = request.Age,
              Gender = request.gender,
              IsActive = true,
              Adress = request.Adress,//5
              PostalCode = request.PostalCode,//6
              DateofBirth = request.DateOfBirth,//7
              PhoneNumber = request.PhoneNumber,//8
              CreatedAt = DateTime.UtcNow,
              AccountStatus = false,
              SubscriptionPlan = true,
              IsAvailable = Availibility.DispoForEmergencysOnly,
              AcceptRequest = true,
              CheckedSchedule = true,
              TwoFactorEnabled = false,
              IsOnline = false,
              LastLogin= DateTime.UtcNow,
              Certif = relativePathCertif,
              identite= relativePath,           
            };

            _context.ProSs.Add(newPro);
            await _context.SaveChangesAsync();
            return new SignUpResultProS
            {
                ProSUID = newPro.UID, // Renvoie l'UID du ProS inscrit
                filename = fileName, // Renvoie le nom du fichier sauvegardé
                filename2 = fileNameCertif // Renvoie le nom du fichier de certificat sauvegardé
            };

        }



        
        // 3️⃣ Déconnexion
        public void Logout(string token)
        {
            _revokedTokens.Add(token);
        }

        public bool IsTokenRevoked(string token)
        {
            return _revokedTokens.Contains(token);
        }

     //TODO:code de la methode pour mettre a jour is validated a true apres que le medecin a valider le docier 










/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////TODO:LES ALGO DE HASH ,SALT,et VERIF PASSWORDTODO://///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://///////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     // Génération du sel

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }


        // Hachage du mot de passe avec SHA-256 + sel
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Vérification du mot de passe
        private bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            return HashPassword(enteredPassword, salt) == storedHash;
        }

           //maskage de l'email
           private string MasquerEmail(string email)
        {
            var parts = email.Split('@');
            if (parts[0].Length <= 2) return "***@" + parts[1];

            return parts[0].Substring(0, 2) + new string('*', 3) + "@" + parts[1];
        } 
     }
}

























































































































//a reutiliser FIXME:

/*var valide = _context.Patients.FirstOrDefault(p => p.IsValidated == validation);

              if (valide == null || !valide.IsValidated) // Vérifie que l'objet n'est pas null et que IsValidated est false
              {
                    return "Compte en attente de validation.";
              }*/





              /*FullName = request.FullName, //1         
                Email = request.Email, //2
                PasswordHash = hashedPassword,//9
                Salt = salt,
                Role = request.Role,//3
                IsActive = true,
                City = request.City,//4
                PostalCode = request.PostalCode, //5
                DateOfBirth = request.DateOfBirth,//6
                PhoneNumber = request.PhoneNumber,//7
                CreatedAt = DateTime.UtcNow,
                LastLogin = null,
                AccountStatus = false,
                TwoFactorEnabled = false,
                SubscriptionPlan = false,
                IsOnline = false,*/


                /* FullName = request.FullName, //1
              Email = request.Email,//2
              PasswordHash = hashedPassword,//3
              Salt = salt,
              Role = request.Role, // Conversion explicite//4
              IsActive = true,
              City = request.City,//5
              PostalCode = request.PostalCode,//6
              DateOfBirth = request.DateOfBirth,//7
              PhoneNumber = request.PhoneNumber,//8
              CreatedAt = DateTime.UtcNow,
              AccountStatus = false,
              SubscriptionPlan = true,
              IsAvailable = true,
              AcceptRequest = true,
              CheckedSchedule = true,
              TwoFactorEnabled = false,
              IsOnline = false,*/

            /* FullName = request.FullName, //1
             Email = request.Email, //2
             PasswordHash = hashedPassword,//3
             Salt = salt,
             Role = request.Role, // 4Conversion explicite
             IsActive = true,
             isAmbulanceReady = false, // Définir la valeur appropriée
             IDCentre = Guid.NewGuid(),// Assurez-vous que cette propriété est définie dans SignUpRequest
             City = request.City,//5
             PostalCode = request.PostalCode,
             DateOfBirth =request.DateOfBirth,//6
             PhoneNumber = request.PhoneNumber,//7
             AccountStatus = false,// Assurez-vous que AccountStatus est défini
             SubscriptionPlan = true, // Définir la valeur appropriée*/