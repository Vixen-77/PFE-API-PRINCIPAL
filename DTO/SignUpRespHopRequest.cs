using APIAPP.Enums;

namespace APIAPP.DTO{

public class SignUpRespHopRequest
{
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string FullName { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public required RoleManager Role { get; set; }
        
}
}
