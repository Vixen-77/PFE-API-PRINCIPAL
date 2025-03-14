
using System.ComponentModel.DataAnnotations;
namespace APIAPP.DTO{
public class SignInRequestProSrespHop
{
    public required int Role { get; set; } // 20 ou 30
    [EmailAddress]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}
}