namespace APIAPP.DTO.SignUpPatientRawRequest;
public class SignUpPatientRawRequest
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Adress { get; set; }
    public required string PostalCode { get; set; }
    public required string DateOfBirth { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Role { get; set; }
    public required IFormFile File { get; set; }
    public required string Age { get; set; }
    public required string Gender { get; set; }
    public required string Weight { get; set; }
    public required string Height { get; set; }
}