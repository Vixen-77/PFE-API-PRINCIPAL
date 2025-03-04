using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAPP.Models
{
    public class RespHop : USER
    {   
        [ForeignKey(nameof(Centre))] 
        public required Guid IdC { get; set; } // la clé etrangère IdC de Centre

    }
}
