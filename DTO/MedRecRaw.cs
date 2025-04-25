namespace APIAPP.DTO{

 public class MedRecRaw{

  public required string ID { get; set; }
  public required string Title {get; set;}
  public required IFormFile file { get; set; }
  public required string MailMedecin { get; set; }
  public required string Description { get; set; }

 }

}