using System.ComponentModel.DataAnnotations;


namespace APIAPP.DTOResponse
{
    public class SignInResultt
    {
        public required string Token { get; set; }   
        public DateTime ExpiresAt { get; set; }
        public required Guid UID {get;set;}
        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string Name {get;set;}
        public required string LastName {get;set;}
        public required string height {get;set;}
        public required string weight {get;set;}
        public required string phonenumber {get;set;}
        public required string postalcode {get;set;}
        public required string address {get;set;}
        public required string birthdate {get;set;}

        public required string pdp {get;set;}
    }
}
