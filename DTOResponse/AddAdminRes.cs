using System.ComponentModel.DataAnnotations;


namespace APIAPP.DTOResponse
{
    public class AddAdminResult
    {
        public required string AdminUID { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Key { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
         
    }
}
