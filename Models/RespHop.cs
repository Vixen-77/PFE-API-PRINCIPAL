using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAPP.Models
{
    public class RespHop : USER
    {
        [Key]
        public required Guid IdResp { get; set; }
        
        [ForeignKey(nameof(Centre))] 
        public required Guid IdC { get; set; }

    }
}
