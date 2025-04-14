using LibrarySSMS.Enums;

namespace APIAPP.DTO{

public class SignUpProSRequest
{
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Name { get; set; }
        public required bool gender {get; set;}
        public required int Age {get; set;}
        public required string LastName {get;set;}
        public required string Adress { get; set; }
        public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public required IFormFile File { get; set; } 
        public required IFormFile FileCertif { get; set; } 
        public required RoleManager Role { get; set; }

}
}