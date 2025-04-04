
using LibrarySSMS;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using LibrarySSMS.Models;
using System.Net.Http.Json;
using System.Text.Json;
using APIAPP.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using APIAPP.Exceptions;
using System.Threading.Tasks;


namespace APIAPP.Services
{
    public class UploadindatabaseService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public UploadindatabaseService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> UpdatePatientFilePath(Guid userId, string filePath)
        {
            var patient = await _context.Patientss.FindAsync(userId);
            if (patient == null)
            {
                throw new AuthException("Aucun patient trouvé avec cet ID.", 401);
            }
            
            patient.MedicalRecordPath = filePath;
            await _context.SaveChangesAsync();
            
            // Envoi d'un e-mail au médecin avec le fichier
            
            return true;
            
        }
    }
}
