using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace APIAPP.DTO
{
    public class SignInRequest
    {

        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
    }
}