using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations.Schema;
 namespace APIAPP.Models{
     public class Proche{                                 

        [Key]
        public int IdP { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }

        // Clé étrangère pour la relation avec Patient
        public int PatientId { get; set; }
        [ForeignKey (nameof(Patient))]
        public required Guid prochId { get; set;}
     }
 }