
using LibrarySSMS.Enums;
namespace APIAPP.DTO{

public class SignUpPatientRequest
{
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Name { get; set; }
        public required string LastName {get;set;}
        public required string City { get; set; }
        
        public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public required RoleManager Role { get; set; } 
        public required string MailMed { get; set;} 
        public required IFormFile File { get; set; } // Ajout du fichier uploadé
        public required int Age {get; set;}
        public required bool Gender {get; set;}   //0 si femme et 1 si homme
        public required double Weight {get; set;} 
        public required double Height {get;set;}

}
}