using System.ComponentModel.DataAnnotations;


namespace APIAPP.DTOResponse
{
    public class SignInResultAdmin
    {
        public required string Token { get; set; }   
        public DateTime ExpiresAt { get; set; }
        public required Guid UID {get;set;}
        public required int Role {get; set;} 
        [EmailAddress]
        public required string Email { get; set; }
        public required string FullName {get;set;}
    }
}
