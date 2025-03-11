using Microsoft.EntityFrameworkCore;
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
}