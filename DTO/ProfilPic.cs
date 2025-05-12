namespace APIAPP.DTO
{

public class ProfilePicDto{

    public required string ID { get; set; }
    public required string Role { get; set; }
    public required IFormFile File { get; set; } 

}


}