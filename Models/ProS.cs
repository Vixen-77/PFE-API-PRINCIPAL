using System.ComponentModel.DataAnnotations;
using APIAPP.Models;
using APIAPP.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAPP.Models
{
    public class ProfSanté
    {
      public required bool IsAvailable { get; set; }
      public required Speciality Spe { get; set; }  

       //voir si on rajouter des attribut ou ideé intéréssante
    }
}
