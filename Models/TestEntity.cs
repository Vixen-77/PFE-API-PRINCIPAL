using System.ComponentModel.DataAnnotations;
using APIAPP.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIAPP.Models
{
    public class TestEntity : USER
    {
        
        [Required]  // Rendre Name obligatoire
        public string Name { get; set; } = string.Empty;

        public UserState Etat { get; set; }
    }
}
