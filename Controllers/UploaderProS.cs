using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIAPP.Models;
using APIAPP.Services;
using APIAPP.DTO;
using APIAPP.Enums;
using Microsoft.AspNetCore.Cors;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class UploaderProS : ControllerBase
{
 [HttpPost("upload")]
 [EnableCors("AllowReactApp")]
public async Task<IActionResult> UploadFile(IFormFile file1,FormFile file2, [FromForm] Guid userId)
{
    if (file1 == null || file1.Length == 0)
        return BadRequest("No file uploaded");

    var filePath = Path.Combine("./DataIDFProfSante", file1.FileName);
    
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file1.CopyToAsync(stream);
    }


//////////////////////////////////////////////////////////////////////////////////////////////////////////////

     if (file2 == null || file2.Length == 0)
        return BadRequest("No file uploaded");

    var filePath2 = Path.Combine("./DataProfSante", file2.FileName); //docier medicale 
    
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file1.CopyToAsync(stream);
    }
     

    return Ok(new { message = "File uploaded successfully", path = filePath });

}
    
}