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



       
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////FIXME: Connexion - Gère les rôles séparément FIXME://////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public string SignInPatient(string email, string password,bool validation)
        {
            var Patient = _context.Patientss.FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
            if (Patient == null || !VerifyPassword(password, Patient.PasswordHash, Patient.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject); 
                 throw new  AuthException("il est possible qu' Aucun patient trouvé avec cet email.",401);
              }
              
                    // Générer le token JWT
                    return _jwtService.GenerateTokenPatient(Patient);
                    
        }

        public string SignInProSante(string email, string password)
        {
            var prosante = _context.ProSs.FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
            if (prosante == null || !VerifyPassword(password, prosante.PasswordHash, prosante.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                throw new AuthException("il est possible qu' Aucun profecionel de santé trouvé avec cet email.",401);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateTokenProS(prosante);
        }

        public string SignInRespoHopital(string email, string password)
        {
            var respHop = _context.RespHops.FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
            if (respHop == null || !VerifyPassword(password, respHop.PasswordHash, respHop.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                throw new AuthException("il est possible qu' Aucun responsable d'hopitale trouvé avec cet email.",401);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateTokenRespHop(respHop);
        }


        
        public string SignInAdmin(string email, string password , Guid key)
        {
         var admin = _context.Admins.FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.PasswordHash == password && p.UIDKEY == key);


          if (admin == null || !VerifyPassword(password, admin.PasswordHash, admin.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                throw new AuthException("il est possible qu'Aucun admin trouvé avec cet email.",401);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateTokenAdmin(admin);
        }


        public string SignInSuperAdmin(string email, string password , Guid key)
        {
         var superadmin = _context.SuperAdmins.FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.PasswordHash == password && p.UIDKEY == key);


          if (superadmin == null || !VerifyPassword(password, superadmin.PasswordHash, superadmin.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                throw new AuthException("il est possible qu'Aucun Super admin trouvé avec cet email.",401);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateTokenSuperAdmin(superadmin);
        }



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////FIXME: Inscription - Gère les rôles séparément FIXME://////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public bool SignUpPatient(SignUpPatientRequest request) // FIXME:
        {
            if (_context.Patientss.Any(p => p.Email.ToLower()== request.Email.ToLower()))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);

            var newPatient = new Patient

            {   
                UID= Guid.NewGuid(),
                Name = request.Name, //1
                LastName = request.LastName, //1
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
                IdProche = Guid.NewGuid(),
                SubscriptionPlan = false,
                IsOnline = false,
                State= UserState.Conducteur, //par defaut mais le user peut le changé ou bien changement par automatisation
                MedicalRecordPath = string.Empty, // Définir le chemin du dossier médical
                MailMed = request.Email, // Définir l'email médical //8
                IsValidated = false,
                IdphoneP = null,
                IdSmartwatchP =null,
                IdSmartwatchNewGenP=null,
                IdVehiculeOBUP=null,
                IdCGMP=null,
                Age =request.Age,
                Gender =request.Gender,  //0 si femme et 1 si homme
                Weight = request.Weight, 
                Height = request.Height,
                Proche = new Proche 
                {
                  Name = null,
                  PhoneNumber = null,
                  IdProche = Guid.NewGuid() // Assurez-vous que cette propriété est correctement définie
                }
            };

            _context.Patientss.Add(newPatient);
            _context.SaveChanges();
            return true;
        }

        public bool SignUpProSante(SignUpProSRequest request)
        {
            if (_context.ProSs.Any(p => p.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);

            var newPro = new ProS

            {
              UID =Guid.NewGuid(),
              FullName = request.FullName, //1
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
              IsOnline = false,
              LastLogin= DateTime.UtcNow,
            };

            _context.ProSs.Add(newPro);
            _context.SaveChanges();
            return true;
        }

        public bool SignUpRespoHopital(SignUpRespHopRequest request)
        {
            if (_context.RespHops.Any(r => r.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);

           var newRespo = new RespHop

        {    
             UID = Guid.NewGuid(),
             FullName = request.FullName, //1
             Email = request.Email, //2
             PasswordHash = hashedPassword,//3
             Salt = salt,
             Role = request.Role, // 4Conversion explicite
             IsActive = true,
             isAmbulanceReady = false, // Définir la valeur appropriée
            // IDCentre = Guid.NewGuid(),//FIXME: a RAJOUTER DANS LA Librérie
             City = request.City,//5
             PostalCode = request.PostalCode,
             DateOfBirth =request.DateOfBirth,//6
             PhoneNumber = request.PhoneNumber,//7
             AccountStatus = false,// Assurez-vous que AccountStatus est défini
             SubscriptionPlan = true, 
             IsOnline = false,
             TwoFactorEnabled =false,
             LastLogin =DateTime.UtcNow,
             CreatedAt =DateTime.UtcNow,
             IdVehiculeOBUSV=null// Définir la valeur appropriée

        };

            _context.RespHops.Add(newRespo);                                                                           
            _context.SaveChanges();
            return true;
            
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

     public bool Confirmvalidation(string email)
     {
         var patient = _context.Patientss.FirstOrDefault(p => p.Email == email);
         if (patient != null)
         {
             patient.IsValidated = true;
             _context.SaveChanges();
             return true; // Return true if validation is successful
         }
         return false; // Return false if no patient is found
     }









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