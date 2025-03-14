
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIAPP.Enums;
namespace APIAPP.Models{
public class RespHop 
{
    
    
    public required bool isAmbulanceReady { get; set; } // true si l'ambulance est prête à partir
    

    public required Guid IDCentre { get; set; }

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
        public string? KeyACC {get; set;}
}
}