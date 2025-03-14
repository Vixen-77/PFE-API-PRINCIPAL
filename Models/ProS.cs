using System.ComponentModel.DataAnnotations;
using APIAPP.Models;
using APIAPP.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAPP.Models
{
    public class ProS 
    {
      public required bool IsAvailable { get; set; }
      public required bool AcceptRequest { get; set; } // true si le proS accepte l'urgence 
      public required bool CheckedSchedule { get; set; } // true si le proS a a consulté toute les alerte 

        public  string? KeyMed { get; set; }


      [Key]
        public Guid UID { get; set; } 
    
        [EmailAddress]
        public required string Email { get; set; }
        
        public required string PasswordHash { get; set; }
        
        public required string Salt { get; set; }

        public required string FullName { get; set; }
        public required string City { get; set; }
         public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
         public required bool AccountStatus  {get; set; }   // false=normale true= user suspendu
        public bool TwoFactorEnabled { get; set; } 
        public required bool  SubscriptionPlan { get; set; }  //false=gratuit true=payant  //avoir si on peut le faire
        public bool IsOnline { get; set; } 
        public required RoleManager Role { get; set; }

        public required bool IsActive { get; set; }
      
      
      


       //voir si on rajouter des attribut ou ideé intéréssante
    }
}
