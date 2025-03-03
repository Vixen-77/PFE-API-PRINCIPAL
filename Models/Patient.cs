using System.ComponentModel.DataAnnotations;
using APIAPP.Models;
using APIAPP.Enums;
using System.Text.Json.Serialization;

namespace APIAPP.Models
{
    public class Patient : USER
    {
        public required Guid prochId { get; set;}
        public required Device ConnectedDevice {get; set;}
        public required UserState State { get; set; }
        [JsonIgnore]
        public required string MedicalRecordPath { get; set; }  //lien du fichier du patient au moment de l'inscription
        public required string MailMed { get; set;} 
        public int? NbSec { get; set;}  //nombre de fois ou il a était secouru (voir si c'est un patient subissant un nombre considérable d'anomalie)
    }
}
