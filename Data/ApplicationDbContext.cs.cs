using Microsoft.EntityFrameworkCore;
using APIAPP.Models; // Vérifie bien le namespace

namespace APIAPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Déclaratuion les tables ici (apres conseption)
        // Table temporaire pour tester la base de données
        public DbSet<TestEntity> TestEntities { get; set; }
    }
}

