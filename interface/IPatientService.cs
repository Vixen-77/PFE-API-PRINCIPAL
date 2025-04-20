

public interface IPatientService
{
    Task<bool> UpdatePatientFilePath(Guid userId, string filePath);
}
    
