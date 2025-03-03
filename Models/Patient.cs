using System.ComponentModel.DataAnnotations;
using APIAPP.Models;
using APIAPP.Enums;

namespace APIAPP.Models
{
    public class Patient : USER
    {

        public required UserState UserState { get; set; }
        public required string MedicalRecordPath { get; set; }  
        public required string MailMed { get; set;} //a proteger
        public List<int> EmergencyContacts { get; set; } = new();
        public int? NbSec { get; set;} 
    }
}
