using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using LibrarySSMS;

namespace APIAPP.Services
{
    public class UploaderPatient
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly UploadindatabaseService _uploaddatabase;
        private readonly ILogger<UploaderPatient> _logger;

        public UploaderPatient(
            IEmailService emailService,
            UploadindatabaseService uploadindatabase,
            AuthService authService,
            AppDbContext dbContext,
            ILogger<UploaderPatient> logger)
        {
            _uploaddatabase = uploadindatabase;
            _emailService = emailService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> SignupAndUpload(IFormFile file, Guid userId)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Fichier non fourni ou vide.");
                return false;
            }

            try
            {
                // Chemin relatif au projet
                var folderName = "Datapatientidf";
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Chemin complet sur le disque
                var filePath = Path.Combine(uploadFolder, file.FileName);

                // Chemin à stocker dans la base (relatif)
                var dbPath = Path.Combine(folderName, file.FileName);

                // Sauvegarde dans la base
                await _uploaddatabase.UpdatePatientFilePath(userId, dbPath);

                // Sauvegarde du fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"Fichier {file.FileName} uploadé avec succès pour l'utilisateur {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload du fichier.");
                return false;
            }
        }
    }
}
