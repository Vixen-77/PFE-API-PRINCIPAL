using System.ComponentModel.DataAnnotations;



namespace APIAPP.DTO
{
    public class Desactivation
    {

        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}