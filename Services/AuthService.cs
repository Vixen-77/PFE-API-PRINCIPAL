//FIXME: This service implemente
//TODO:1-Sign in,
//TODO:2-Sign up , 
//TODO:3-Logout 
//TODO:4-desactivated account 
//TODO:5-JWT call


/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////TODO:RVOIR TOUTE LES ERREURFIXME:FIXME:FIXME:TODO://///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO:TODO://///////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




using APIAPP.Models;
using APIAPP.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using APIAPP.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using APIAPP.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace APIAPP.Services
{
    public class AuthService
    {   //FIXME: jwt a un probleme a reguler le plus vite possible 
        private readonly ApplicationDbContext _context;
        private readonly JWTService _jwtService;
        private readonly HashSet<string> _revokedTokens = new HashSet<string>();

        public AuthService(ApplicationDbContext context, JWTService jwtService)
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
            var patient = _context.Patients.FirstOrDefault(p => p.Email == email);
            if (patient == null || !VerifyPassword(password, patient.PasswordHash, patient.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject); 
              }
              var valide =_context.Patients.FirstOrDefault(p =>p.IsValidated ==validation );
              if (patient != null && patient.isValidated == false)

              {
                return "Compte en attente de validation.";
              }
              else
              {
                    // Générer le token JWT
                  return _jwtService.GenerateToken(patient);
              }
        }

        public string SignInProSante(string email, string password)
        {
            var prosante = _context.ProSs.FirstOrDefault(p => p.Email == email);
            if (prosante == null || !VerifyPassword(password, prosante.PasswordHash, prosante.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateToken(prosante);
        }

        public string SignInRespoHopital(string email, string password)
        {
            var respHop = _context.RespHops.FirstOrDefault(p => p.Email == email);
            if (respHop == null || !VerifyPassword(password, respHop.PasswordHash, respHop.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateToken(respHop);
        }






/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////FIXME: Inscription - Gère les rôles séparément FIXME://////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public bool SignUpPatient(SignUpPatientRequest request) // FIXME:
        {
            if (_context.Patients.Any(p => p.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.PasswordHash, salt);

            var newPatient = new Patient
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Salt = salt,
                Role = request.Role,
                IsActive = true,
                City = request.City,
                PostalCode = request.PostalCode,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                LastLogin = null,
                AccountStatus = false,
                TwoFactorEnabled = false,
                SubscriptionPlan = false,
                IsOnline = false,
                State= UserState.Pieton, //par defaut mais le user peut le changé ou bien changement par automatisation
                MedicalRecordPath = "path/to/medical/record", // Définir le chemin du dossier médical
                IdentityRecordPath = "path/to/identity/record", // Définir le chemin du dossier d'identité
                MailMed = request.Email, // Définir l'email médical
                IdProche = Guid.NewGuid(), // Définir l'ID du proche
                ValidationToken = 
                IsValidated = false,
                // Initialiser l'objet Proche
                Proche = new Proche 
               {
                  Name = null,
                  PhoneNumber = null,
                  IdProche = Guid.NewGuid() // Assurez-vous que cette propriété est correctement définie
                }
            };

            _context.Patients.Add(newPatient);
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
              FullName = request.FullName,
              Email = request.Email,
              PasswordHash = hashedPassword,
              Salt = salt,
              Role = request.Role, // Conversion explicite
              IsActive = true,
              City = request.City,
              PostalCode = request.PostalCode,
              PhoneNumber = request.PhoneNumber,
              AccountStatus = false,
              SubscriptionPlan = true,
              IsAvailable = true,
              AcceptRequest = true,
              CheckedSchedule = true,
              IdentityDiplome = "path/to/diplome", // Définir le chemin du diplôme
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
             FullName = request.FullName,
             Email = request.Email,
             PasswordHash = hashedPassword,
             Salt = salt,
             Role = RoleManager.RespHop, // Conversion explicite
             IsActive = true,
             isAmbulanceReady = false, // Définir la valeur appropriée
             IDCentre = Guid.NewGuid(),// Assurez-vous que cette propriété est définie dans SignUpRequest
             City = request.City,
             PostalCode = request.PostalCode,
             PhoneNumber = request.PhoneNumber,
             AccountStatus = false,// Assurez-vous que AccountStatus est défini
             SubscriptionPlan = true, // Définir la valeur appropriée
             KeyACC = "some-key" // Définir la valeur appropriée
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

        // 4️⃣ Désactivation du compte
        /*public bool DesactivateAccount(Desactivation request) //FIXME:


        {
          
          
          
            //faire un switch 
        }*/















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
