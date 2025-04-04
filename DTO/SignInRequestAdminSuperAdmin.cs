using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace APIAPP.DTO
{
    public class SignInRequestAdminSuperAdmin
    {

        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required Guid UIDKEY {get; set;}
    }
}