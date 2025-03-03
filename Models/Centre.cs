using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAPP.Models
{
    public class Centre 
    {
        [Key]
        public required Guid IdC { get; set; }

        [ForeignKey(nameof(RespHop))] 
        public required Guid IdResp { get; set; }

        public required string NomC { get; set; }
        public required string Adresse { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public int Nbamb { get; set; }

    }
}
