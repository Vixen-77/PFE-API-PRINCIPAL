using System.ComponentModel.DataAnnotations;

namespace APIAPP.Models
{
    public class TestEntity
    {
        public int Id { get; set; }

        [Required]  // Rendre Name obligatoire
        public string Name { get; set; } = string.Empty;
    }
}
