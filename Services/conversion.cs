
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
using APIAPP.DTO.SignUpPatientRawRequest;
using APIAPP.DTO.SignInRaw;
using APIAPP.DTO.SignUpProSRawRequest;

namespace APIAPP.Services
{
    public class ConversionService
    {
        public SignUpPatientRequest ToTyped(SignUpPatientRawRequest data)
{
    return new SignUpPatientRequest
    {
        Email = data.Email,
        PasswordHash = data.PasswordHash,
        Name = data.Name,
        LastName = data.LastName,
        Adress = data.Adress,
        PostalCode = data.PostalCode,
        PhoneNumber = data.PhoneNumber,
        Role = 10,
        Age = int.TryParse(data.Age, out var age) ? age : 0,
        Gender = data.Gender == "1",
        Weight = double.TryParse(data.Weight.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var weight) ? weight : 0,
        Height = double.TryParse(data.Height.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var height) ? height : 0,
        DateOfBirth = DateTime.TryParse(data.DateOfBirth, out var dob) ? dob : DateTime.MinValue,
        File = data.File
    };
}

    public SignInRequest ToRaw(SignInRaw data){
        return new SignInRequest
        {
            Email = data.Email,
            PasswordHash = data.PasswordHash,
            Role = int.TryParse(data.Role, out var role) ? role : 0
        };
    }

    public SignUpProSRequest ToTypedProS(SignUpProSRawRequest data){
        return new SignUpProSRequest
        {
        Email = data.Email,
        PasswordHash = data.PasswordHash,
        Name = data.Name,
        LastName = data.LastName,
        Adress = data.Adress,
        PostalCode = data.PostalCode,
        PhoneNumber = data.PhoneNumber,
        Role = 10,
        Age = int.TryParse(data.Age, out var age) ? age : 0,
        gender = data.gender == "1",
        DateOfBirth = DateTime.TryParse(data.DateOfBirth, out var dob) ? dob : DateTime.MinValue,
        File = data.File,
        FileCertif = data.FileCertif,
        };
    }

    }
}