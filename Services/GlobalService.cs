using LibrarySSMS;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Collections.Generic;
using LibrarySSMS.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using APIAPP.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using APIAPP.Exceptions;
using Microsoft.AspNetCore.Antiforgery;
using Azure.Core;
using LibrarySSMS.Models;
using System.Linq;
using System.Threading.Tasks;
using APIAPP.DTOResponse;
using APIAPP.Services;
using System.Collections.Concurrent;

public class GlobalService
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public GlobalService(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }
   

       public async Task<bool> VerifyPasswordPatient(string email)
      {
       var patient = await _context.Patientss
        .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());

        if (patient == null)
       {
         return false;
       }
       return true;
      }      





       public async Task<bool> VerifyPasswordProS(string email)
      {
       var pros = await _context.ProSs
        .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());

        if (pros == null )
       {
        return false;
       }
       return true;
       }





    private static readonly ConcurrentDictionary<string, (string Code, DateTime Expiry)> codes = new();
          public string GenerateCode(string email)
        {
        // Supprimer les anciens codes expirés
        CleanExpiredCodes();

        var random = new Random();
        string code = random.Next(100000, 999999).ToString();

        var expiration = DateTime.UtcNow.AddMinutes(30);

        // On écrase s'il existe déjà un code pour cet email
        codes[email.ToLower()] = (code, expiration);

        return code;
    }





    private void CleanExpiredCodes()
    {
        var now = DateTime.UtcNow;
        foreach (var key in codes.Keys.ToList())
        {
            if (codes[key].Expiry < now)
            {
                codes.TryRemove(key, out _);
            }
        }
    }




        public bool ValidateCode(string email, string code)
    {
        if (codes.TryGetValue(email.ToLower(), out var stored))
        {
            if (stored.Code == code && DateTime.UtcNow <= stored.Expiry)
            {
                codes.TryRemove(email.ToLower(), out _); // Supprimer après usage
                return true;
            }
        }

        return false;
    }


  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     /// ///////////////////////////////////////////////////////////FIXME:pour les Numero de etelphone //FIXME:////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     ///  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private static readonly ConcurrentDictionary<string, (string Code, DateTime Expiry)> codes1 = new();

   public string GenerateCodePhoneNumber(string phoneNumber)
    {
    // Supprimer les anciens codes expirés du dictionnaire des numéros
    CleanExpiredPhoneCodes();

    var random = new Random();
    string code = random.Next(100000, 999999).ToString();
    var expiration = DateTime.UtcNow.AddMinutes(30);

    // On stocke le code avec sa date d'expiration
    codes1[phoneNumber] = (code, expiration);

    return code;
  }

  private void CleanExpiredPhoneCodes()
  {
    var now = DateTime.UtcNow;

    foreach (var entry in codes1)
    {
        if (entry.Value.Expiry < now)
        {
            codes1.TryRemove(entry.Key, out _);
        }
    }
  }

public bool ValidateCodePhoneNumber(string phoneNumber, string code)
{
    if (codes1.TryGetValue(phoneNumber, out var stored))
    {
        if (stored.Code == code && DateTime.UtcNow <= stored.Expiry)
        {
            codes1.TryRemove(phoneNumber, out _); // Supprimer après usage
            return true;
        }
    }

    return false;
}



}

















/*Task RequestReset(string email, string oldPassword, string uid, string role)
Task SubmitCode(string email, string code)
Task UpdatePassword(string email, string newPassword)

*/

//in React 
/*import { useNavigate } from "react-router-dom";

const navigate = useNavigate();

connection.on("PasswordIncorrect", (msg) => {
  alert(msg);
});

connection.on("CodeSent", () => {
  navigate("/enter-code");
});

connection.on("CodeValid", () => {
  navigate("/new-password");
});

connection.on("PasswordUpdated", () => {
  navigate("/login");
});
*/


