using System.ComponentModel.DataAnnotations;


namespace APIAPP.DTOResponse
{
    public class InfoAlert
    {
        public required int State { get; set; }
        public string? Description { get; set; }
        public required string PatientID { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string height { get; set; }
        public required string weight { get; set; }
        public required string phonenumber { get; set; }
        public required string Gender { get; set; }
        public required string postalcode { get; set; }
        public required string address { get; set; }
        public required string birthdate { get; set; }
        public required string latitudePatient { get; set; }
        public required string longitudePatient { get; set; }
        public string? Location { get; set; }
        public string? ProSID { get; set; }
        public string? firstnamepro { get; set; }
        public string? lastnamepro { get; set; }
        public string? phonenumberProS { get; set; }
        public string? emailProS { get; set; }
        public string? latitudeProS { get; set; }
        public string? longitudeProS { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
