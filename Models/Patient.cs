using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIAPP.Models;
using APIAPP.Enums;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Azure.Identity;

namespace APIAPP.Models
{
    public class Patient 
    {
        
        
        public  string? MedicalRecordPath { get; set; } //lien du fichier du patient au moment de l'inscription
        [EmailAddress]
        public required string MailMed { get; set;} 
        public int? NbSec { get; set;} 
        [ForeignKey(nameof(Proche))]
        public required Guid IdProche { get; set; } // la clé etrangère Id

        public required UserState State { get; set; } // l'état du patient

       
        public virtual required Proche Proche { get; set; } // navigation property
        [Key]
        public required Guid UID { get; set; } 
    
        [EmailAddress]
        public required string Email { get; set; }
        
        public required string PasswordHash { get; set; }
        
        public required string Salt { get; set; }

        public required string FullName { get; set; }
        public required string City { get; set; }
         public required string PostalCode { get; set; }
         public required DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
         public required bool AccountStatus  {get; set; }   // false=normale true= user suspendu
        public bool TwoFactorEnabled { get; set; } 
        public required bool  SubscriptionPlan { get; set; }  //false=gratuit true=payant  //avoir si on peut le faire
        public bool IsOnline { get; set; } 
        public required RoleManager Role { get; set; }

        public required bool IsActive { get; set; }
         public bool IsValidated { get; set; }  // Par défaut : false et se mettra a true plus tard 
        
        
    }
}
