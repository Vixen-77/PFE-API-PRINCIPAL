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
        
        public DbSet<TestEntity> TestEntities { get; set; }
       
    }   
}

