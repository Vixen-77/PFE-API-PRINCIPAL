//FIXME: This service implemente
//TODO:1-Sign in,
//TODO:2-Sign up , 
//TODO:3-Logout 
//TODO:4-desactivated account 
/*using APIAPP.Models;
using APIAPP.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using APIAPP.Services;
using APIAPP.Enums;
using System.Net.Http.Json;
using System.Text.Json;
namespace APIAPP.Services

{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly HashSet<string> _revokedTokens = new HashSet<string>();

        public AuthService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // 1️⃣ Connexion - Gère les rôles séparément
        public string SignInPatient(string email, string password)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Email == email);
            if (patient == null || !VerifyPassword(password, patient.PasswordHash, patient.Salt))
              {
                var myObject = new { singninfailure = "Email ou mot de passe incorrect." };
                var jsonString = JsonSerializer.Serialize(myObject);
                
              }

                // Générer le token JWT
                return _jwtService.GenerateToken(patient);
        }

        public string SignInProSante(string email, string password)
        {
            var proS = _context.ProfSs.FirstOrDefault(p => p.Email == email);
            if (proS == null || !VerifyPassword(password, proS.PasswordHash, proS.Salt))
                return null;

            return _jwtService.GenerateToken(proS);
        }

        public string SignInRespoHopital(string email, string password)
        {
            var respo = _context.RespHops.FirstOrDefault(r => r.Email == email);
            if (respo == null || !VerifyPassword(password, respo.PasswordHash, respo.Salt))
                return null;

            return _jwtService.GenerateToken(respo);
        }

        // 2️⃣ Inscription - Gère les rôles séparément
        public bool SignUpPatient(SignUpRequest request)
        {
            if (_context.Patients.Any(p => p.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.Password, salt);

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
                State = request.State,
                MedicalRecordPath = "path/to/medical/record", // Définir le chemin du dossier médical
                IdentityRecordPath = "path/to/identity/record", // Définir le chemin du dossier d'identité
                MailMed = "med@example.com", // Définir l'email médical
                IdProche = Guid.NewGuid(), // Définir l'ID du proche
                Proche = new Proche // Initialiser l'objet Proche
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

        public bool SignUpProSante(SignUpRequest request)
        {
            if (_context.ProSs.Any(p => p.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.Password, salt);

            var newPro = new ProS
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Salt = salt,
                Role = "ProSante",
                IsActive = true
            };

            _context.ProfessionnelsSante.Add(newPro);
            _context.SaveChanges();
            return true;
        }

        public bool SignUpRespoHopital(SignUpRequest request)
        {
            if (_context.ResponsablesHopital.Any(r => r.Email == request.Email))
                return false;

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(request.Password, salt);

            var newRespo = new RespHop
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Salt = salt,
                Role = "RespoHopital",
                IsActive = true
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
        public bool DesactivateAccount(string email)
        {
            var user = _context.USERs.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return false;

            user.IsActive = false;
            _context.SaveChanges();
            return true;
        }

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
}*/
