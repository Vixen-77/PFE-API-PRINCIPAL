namespace APIAPP.DTO{

 public class ProcheAdd{

  public string? ProcheID { get; set; }  
  public required string PatientUID { get; set; }
  public required string PhoneNumber{ get; set; }
  public required string Name { get; set; }
 }

}