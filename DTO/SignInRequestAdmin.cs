using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace APIAPP.DTO
{
    public class SignInRequestAdmin
    {

        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string UIDKEY {get; set;}
    }
}