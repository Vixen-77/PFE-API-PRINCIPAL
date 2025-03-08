using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RestSharp;
using APIAPP.Enums;
using Microsoft.Identity.Client;
namespace APIAPP.Models
{
    public abstract class USER
    {
        [Key]
        public Guid UID { get; set; } 
        public required string Username { get; set; }
        [EmailAddress,JsonIgnore]
        public required string Email { get; set; }
        
        [JsonIgnore]
        public required string PasswordHash { get; set; }
        
        [JsonIgnore]
        public required string Salt { get; set; }

        public required string FullName { get; set; }
        public required string City { get; set; }
         public required string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }

        [JsonIgnore]
        public required string PhoneNumber { get; set; } // Optionnel
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
         public required bool AccountStatus  {get; set; }   // false=normale true= user suspendu
        public bool TwoFactorEnabled { get; set; } 
        public required Device Device { get; set; } 
        public required bool  SubscriptionPlan { get; set; }  //false=gratuit true=payant  //avoir si on peut le faire
        public bool IsOnline { get; set; } 
    }
}