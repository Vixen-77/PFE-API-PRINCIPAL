using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations.Schema;
 namespace APIAPP.Models{
     public class Proche{                                 

        [Key]
        public required Guid IdProche { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

     }
 }