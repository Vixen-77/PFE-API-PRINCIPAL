using System.ComponentModel.DataAnnotations;

namespace APIAPP.Models
{
    public class GérantEM : USER
    {
        [Key]
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string PostalCode { get; set; }
        public required string Role { get; set; }       
    }
}
