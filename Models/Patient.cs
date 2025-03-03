using System.ComponentModel.DataAnnotations;
using APIAPP.Models;
using APIAPP.Enums;

namespace APIAPP.Models
{
    public class Patient : USER
    {
        [Key]
        public required int Id { get; set; }
        public required string Name { get; set; }
        private string PasswordHash { get; set;}
        public new DateTime DateOfBirth { get; set; }
        public required UserState UserState { get; set; }
        public required string MedicalRecordPath { get; set; }  
    }
}
