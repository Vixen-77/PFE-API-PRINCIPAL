using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RestSharp;
using APIAPP.Enums; 

namespace APIAPP.Models
{
    public abstract class USER
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Email { get; set; } //a proteger
        //ajout de l adresse ?
        public required string PasswordHash { get; set;}
        public required string Salt { get; set; }
        public required string FullName { get; set; }
        public required string City { get; set; }
         public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; } // Optionnel
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public required AccountStatus AccountStatus { get; set; } 
        public bool TwoFactorEnabled { get; set; }
        public required Device Device { get; set; } = new();
        public required bool  SubscriptionPlan { get; set; }  //false=gratuit true=payant  //avoir si on peut le faire
        public bool IsOnline { get; set; } 
        
    }

}