using System.ComponentModel.DataAnnotations;

namespace APIAPP.Models
{
    public class Centre
    {
        [Key]
        public required Guid IdC { get; set; } // UID du centre

        public required string NomC { get; set; }
        public required string Adresse { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public int Nbamb { get; set; }
    }
}
/*using Microsoft.EntityFrameworkCore;
using APIAPP.Models; 
using APIAPP.Enums;// Vérifie bien le namespace

namespace APIAPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        

        // Déclaratuion les tables ici (apres conseption)
        // Table temporaire pour tester la base de donnée<
        

        public DbSet<Patient> Patients { get; set; }
        public DbSet<RespHop> RespHops { get; set; }
        public DbSet<Proche> Proches { get; set; }
        public DbSet<Centre> Centres { get; set; }
        public DbSet<ProS> ProSs { get; set; }
       
    }   
}*/