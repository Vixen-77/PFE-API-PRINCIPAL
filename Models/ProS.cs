using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIAPP.Models;
using APIAPP.Enums;

namespace APIAPP.Models
{
    public class ProS : USER
    {
        public required UserState State { get; set; }
        public required bool Dispo { get; set;}
        
    }
}
