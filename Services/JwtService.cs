using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LibrarySSMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
public class JWTService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;


     public JWTService(IConfiguration config)
    {
        _secretKey = config["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
        _issuer    = config["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
        _audience  = config["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");
    }



        public string GenerateTokenPatient(Patient patient)
    {
        return GenerateToken(patient.UID.ToString(), patient.Email, "Patient");
    }

    public string GenerateTokenProS(ProS pros)
    {
        return GenerateToken(pros.UID.ToString(), pros.Email, "ProSante");
    }

    public string GenerateTokenRespHop(RespHop respHop)
    {
        return GenerateToken(respHop.UID.ToString(), respHop.Email, "RespoHopital");
    }

    public string GenerateTokenAdmin(Admin admin)
    {
        return GenerateToken(admin.UIDKEY.ToString(), admin.Email, "Admin");
    }

    
    public string GenerateTokenSuperAdmin(SuperAdmin superadmin)
    {
        return GenerateToken(superadmin.UIDKEY.ToString(), superadmin.Email, "SuperAdmin");
    }
    

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
    private string GenerateToken(string userId, string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID unique du token
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
