using System.ComponentModel.DataAnnotations;


namespace APIAPP.DTOResponse
{
    public class SignInResult
    {
        public required string Token { get; set; }   
        public DateTime ExpiresAt { get; set; }
        public required Guid UID {get;set;}
        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string Name {get;set;}
        public required string LastName {get;set;}
    }
}
//redjahyousra6@gmail.com
//red********@gmail.com 