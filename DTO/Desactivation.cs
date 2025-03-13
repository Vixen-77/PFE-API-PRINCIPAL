using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIAPP.Enums;


namespace APIAPP.Models
{
    public class Desactivation
    {

        public required RoleManager Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}